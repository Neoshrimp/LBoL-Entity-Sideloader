using BepInEx;
using HarmonyLib;
using LBoL.Base.Extensions;
using LBoLEntitySideloader.Attributes;
using LBoLEntitySideloader.Entities;
using LBoLEntitySideloader.Reflection;
using LBoLEntitySideloader.ReflectionHelpers;
using LBoLEntitySideloader.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LBoLEntitySideloader
{
    public class EntityManager
    {
        static private EntityManager _instance;

        private static readonly BepInEx.Logging.ManualLogSource log = BepinexPlugin.log;

        public static EntityManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new EntityManager();
                return _instance;
            }
        }

        public static UserInfo ScanAssembly(Assembly assembly)
        {



            var userInfo = new UserInfo();
            userInfo.assembly = assembly;

            Log.LogDev()?.LogInfo($"Scanning {assembly.GetName().Name}...");

            var exportedTypes = assembly.GetExportedTypes();

            userInfo.assembly = assembly;

            var foundEntityLogicForDefinitionTypes = new HashSet<Type>();

            foreach (var type in exportedTypes)
            {

                if (type.IsSubclassOf(typeof(BaseUnityPlugin)))
                {
                    var attributes = type.GetCustomAttributes(inherit: false);
                    
                    if (type.SingularAttribute<BepInPlugin>(attributes) is BepInPlugin bp)
                    {
                        userInfo.GUID = bp.GUID;
                    }
                    else
                    {
                        log.LogError($"{assembly.GetName().Name}: {type} does not have {typeof(BepInPlugin).Name} attribute despite extending {typeof(BaseUnityPlugin).Name}");
                    }

                    var bepinDependencies = type.MultiAttribute<BepInDependency>(attributes);

                    if (bepinDependencies == null || !bepinDependencies.Any(bd => bd.DependencyGUID == PluginInfo.GUID && bd.Flags == BepInDependency.DependencyFlags.HardDependency))
                    {
                        log.LogWarning($"{assembly.GetName().Name}: {type} does not have a {typeof(BepInDependency).Name} attribute with {PluginInfo.GUID} as hard dependency.");
                    }
                    continue;
                }


                if (type.IsSubclassOf(typeof(EntityDefinition)))
                {
                    // final templates need to be Sealed
                    if (type.IsSealed)
                    {

                        var definition = (EntityDefinition)Activator.CreateInstance(type);

                        userInfo.definitionInfos.Add(type, definition);

                        definition.assembly = userInfo.assembly;


                        var overwrite = type.SingularAttribute<OverwriteVanilla>();


                        // 2do add optional DontLoad attribute filter
                        // 2do make sure same component isn't overwritten twice
                        if (overwrite != null)
                        {
                            userInfo.entitiesToOverwrite.Add(type, new ModificationInfo() { attribute = overwrite});
                        }
                    }
                    else if (BepinexPlugin.devModeConfig.Value && !type.IsSealed)
                    {
                        Log.LogDevExtra()?.LogWarning($"(Extra logging) {assembly.GetName().Name}: {type} is subtype of {typeof(EntityDefinition).Name} but isn't sealed. Final entity templates need to be sealed.");
                    }
                    continue;
                }
              

                var facType = TypeFactoryReflection.factoryTypes.FirstOrDefault(t => type.IsSubclassOf(t));
                if (facType != null)
                {
                    if (type.IsSealed)
                    {
                        userInfo.entityInfos.TryAdd(facType, new List<EntityInfo>());

                        var entityLogic = type.SingularAttribute<EntityLogic>();

                        if (entityLogic is null)
                        {
                            log.LogError($"{assembly.GetName().Name}: {type.Name} does not have {typeof(EntityLogic).Name} attribute despite having qualities of an entity logic type. Please add {typeof(EntityLogic).Name} attribute.");
                        }
                        else
                        {
                            if (foundEntityLogicForDefinitionTypes.Contains(entityLogic.DefinitionType))
                            {
                                log.LogError($"{assembly.GetName().Name}: {entityLogic.DefinitionType} already has an entity logic type associated. Entity can only have one type defining its logic. Please remove {typeof(EntityLogic).Name} attribute.");
                            }
                            else
                            {
                                
                                foundEntityLogicForDefinitionTypes.Add(entityLogic.DefinitionType);

                                var entityInfo = new EntityInfo(facType, type, entityLogic.DefinitionType);
                                userInfo.entityInfos[facType].Add(entityInfo);

                                userInfo.definition2EntityLogicType.Add(entityLogic.DefinitionType, entityInfo.entityType);
                            }

                        }
                    }
                    else if (BepinexPlugin.devModeConfig.Value && !type.IsSealed)
                    {
                        Log.LogDevExtra()?.LogWarning($"(Extra logging) {assembly.GetName().Name}: {type} is subtype of {facType.Name} but isn't sealed. Final entity logic types need to be sealed");
                    }
                    continue;
                }
            }


            if (BepinexPlugin.devModeConfig.Value && BepinexPlugin.devExtraLoggingConfig.Value)
            {
                // all definitions needs to be instantiated at the point of this check
                foreach (var ed in foundEntityLogicForDefinitionTypes)
                {
                  if(userInfo.definition2EntityLogicType.TryGetValue(ed, out Type entityLogicType) && userInfo.definitionInfos.TryGetValue(ed, out EntityDefinition definition))

                    if (!entityLogicType.IsSubclassOf(definition.EntityType()))
                    {
                        throw new InvalidProgramException($"(Extra Logging) {ed.Name} expects its entity logic type, {entityLogicType.Name}, to extend {definition.EntityType()}. Instead {entityLogicType.Name} extends {entityLogicType.BaseType} ");
                    }
                }

                foreach (var kv in userInfo.definitionInfos)
                {
                    var defType = kv.Key;
                    var defVal = kv.Value;

                    if (!userInfo.IsForOverwriting(defType) && TemplatesReflection.ExpectsEntityLogic(defType) && !foundEntityLogicForDefinitionTypes.Contains(defType))
                    {
                        log.LogError($"(Extra logging) {defType.Name} needs entity logic type extending {defVal.EntityType().Name} but none was found. Did you define public sealed entity logic class with {typeof(EntityLogic).Name} attribute?");
                    }
                }
            }

            log.LogMessage($"{assembly.GetName().Name} scanned! {userInfo.definitionInfos.Count()} Entity definition(s) found.");

            if (BepinexPlugin.devModeConfig.Value && BepinexPlugin.devExtraLoggingConfig.Value)
            {
                log.LogInfo("(Extra logging) Entity definitions found: ");
                userInfo.definitionInfos.Do(kv => log.LogInfo(kv.Key.Name));
            }

            return userInfo;
        }

        internal class SideloaderUsers
        {
            public Dictionary<Assembly, UserInfo> userInfos = new Dictionary<Assembly, UserInfo>();

            
            public void AddUser(Assembly assembly)
            {
                if (userInfos.ContainsKey(assembly))
                {
                    throw new Exception($"{assembly.GetName().Name} is already registered");

                }
                try
                {
                    userInfos.Add(assembly, ScanAssembly(assembly));

                }
                catch (Exception ex)
                {

                    log.LogError($"{assembly.GetName().Name}: {ex}");
                }

            }

/*            public void UnregisterUser(Assembly assembly)
            {
                if (userInfos.ContainsKey(assembly))
                {
                    userInfos[assembly] = new UserInfo();
                }

            }

            public void ReRegisterUser(Assembly assembly)
            {
                if (userInfos.ContainsKey(assembly)) 
                {
                    userInfos[assembly] = ScanAssembly(assembly);
                }
            }*/



            public Type GetEntityLogicType(Assembly assembly, Type definitionType)
            {
                if (this.userInfos.TryGetValue(assembly, out UserInfo user) && user.definition2EntityLogicType.TryGetValue(definitionType, out Type entityType))
                {
                    return entityType;
                }

                throw new ArgumentException($"{definitionType.Name} was not found in {assembly.GetName().Name}");
            }

        }

        internal SideloaderUsers sideloaderUsers = new SideloaderUsers();


        

        static public void RegisterSelf()
        {
            var a = Assembly.GetCallingAssembly();
            RegisterAssembly(a);
        }

        static public void RegisterAssembly(Assembly assembly)
        {
            Instance.sideloaderUsers.AddUser(assembly);
        }


        internal bool RegisterId(UserInfo user, EntityDefinition entityDefinition)
        {

            Log.LogDev()?.LogDebug($"Registering id:  template: {entityDefinition.GetType().Name}, id: {entityDefinition.GetId()}, IsForOverwriting: {user.IsForOverwriting(entityDefinition.GetType())}");

            var definitionType = entityDefinition.GetType();
            try
            {
                if (user.entitiesToOverwrite.ContainsKey(definitionType))
                {
                    if (!UniqueTracker.Instance.id2ConfigListIndex[entityDefinition.ConfigType()].ContainsKey(entityDefinition.GetId()))

                    {
                        log.LogError($"RegisterId: {entityDefinition.GetId()} was not found among vanilla ids. Overwriting is not supported for non-vanilla entities (yet, maybe).");
                        UniqueTracker.Instance.invalidRegistrations.Add(definitionType);
                        return false;
                    }
                    return true;
                }

                UniqueTracker.AddUniqueId(entityDefinition, user);
            }
            catch (Exception ex)
            {

                log.LogError(ex);
                UniqueTracker.Instance.invalidRegistrations.Add(definitionType);
                return false;
            }


            return true;

        }


        internal void RegisterConfig<C>(IConfigProvider<C> configProvider, UserInfo user, EntityDefinition entityDefinition = null) where C : class
        {

            if (entityDefinition == null)
            {
                entityDefinition = (EntityDefinition)configProvider;
            }


            try
            {
                var configType = entityDefinition.ConfigType();
                var newConfig = configProvider.MakeConfig();

                var f_Id = ConfigReflection.GetIdField(configType);

                switch (entityDefinition.UniqueId.idType)
                {
                    case IdContainer.IdType.String:
                        f_Id.SetValue(newConfig, (string)entityDefinition.UniqueId);
                        break;
                    case IdContainer.IdType.Int:
                        f_Id.SetValue(newConfig, (int)entityDefinition.UniqueId);
                        break;
                    default:
                        log.LogWarning("RegisterConfig: you shouldn't be here");
                        break;
                }
                


                Log.LogDev()?.LogDebug($"Registering config: id: {entityDefinition.UniqueId}, config type:{entityDefinition.ConfigType().Name}");



                // For adding config to array
                var f_Data = ConfigReflection.GetArrayField(configType);

                var ref_Data = AccessTools.StaticFieldRefAccess<C[]>(f_Data);
                
                // For adding config to dictionary
                var f_IdTable = ConfigReflection.GetTableField(configType);

                if (!user.IsForOverwriting(entityDefinition.GetType()))
                {
                    var f_Index = ConfigReflection.HasIndex(configType);
                    if (f_Index != null)
                    {
                        f_Index.SetValue(newConfig, UniqueTracker.AddUniqueIndex(IdContainer.CastFromObject(f_Index.GetValue(newConfig)), entityDefinition));
                    }
                    ((Dictionary<string, C>)f_IdTable.GetValue(null)).Add(entityDefinition.UniqueId, newConfig);
                    ref_Data() = ref_Data().AddToArray(newConfig).ToArray();
                }
                else
                {
                    var i = UniqueTracker.Instance.id2ConfigListIndex[configType][IdContainer.CastFromObject(f_Id.GetValue(newConfig))];
                    ((Dictionary<string, C>)f_IdTable.GetValue(null)).AlwaysAdd(entityDefinition.UniqueId, newConfig);
                    ref_Data()[i] = newConfig;

                }


            }
            catch (Exception ex)
            {
                log.LogError($"Exception registering config of {entityDefinition}: {ex}");
            }
        }

       

        internal static void RegisterTypes(Type facType, UserInfo user)
        {
           
            var hasTypes = user.entityInfos.TryGetValue(facType, out List<EntityInfo> typesToRegister);

            if (hasTypes)
            {
                Log.LogDev()?.LogInfo($"Registering entity logic types from assembly: {user.assembly.GetName().Name}");
                foreach (var ei in typesToRegister)
                {
                    try
                    {
                        Log.LogDev()?.LogDebug($"Registering entity logic type in TypeFactory<{facType.Name}>, typeName: {ei.entityType.Name}, from template: {ei.definitionType.Name}");

                        if (UniqueTracker.Instance.invalidRegistrations.Contains(ei.definitionType) || !user.definitionInfos.ContainsKey(ei.definitionType))
                        {
                            log.LogError($"TypeFactory<{facType.Name}>: Cannot register entity logic {ei.entityType.Name} because template {ei.definitionType.Name} was not properly loaded.");
                            // entity could be removed from typesToRegister
                            continue;
                        }
                        // should be type.name for now
                        var definition = user.definitionInfos[ei.definitionType];
                        var uId = definition.UniqueId;

                        if (uId != ei.entityType.Name)
                        {

                            log.LogError($"{user.GUID} entity id, {uId}, mismatches entity type name, {ei.entityType.Name}");
                            continue;
                        }


                        
                        if (!user.IsForOverwriting(ei.definitionType))
                        {
                            if (!TypeFactoryReflection.GetAccessRef(facType, TypeFactoryReflection.TableFieldName.FullNameTypeDict)().TryAdd(ei.entityType.FullName, ei.entityType))
                            {
                                log.LogError($"RegisterType: {ei.entityType.Name} matches an already registered type. Please change plugin namespace.");
                            }
                        }
                        else
                        {


                            var id = definition.GetId();

                            var originalType = TypeFactoryReflection.GetAccessRef(facType, TypeFactoryReflection.TableFieldName.TypeDict)()[uId];

                            user.typeName2VanillaType.Add(id, originalType);

                            TypeFactoryReflection.GetAccessRef(facType, TypeFactoryReflection.TableFieldName.FullNameTypeDict)()[originalType.FullName] = ei.entityType;

                        }



                        if (!user.IsForOverwriting(ei.definitionType))

                            TypeFactoryReflection.GetAccessRef(facType, TypeFactoryReflection.TableFieldName.TypeDict)().Add(uId, ei.entityType);
                        else
                            TypeFactoryReflection.GetAccessRef(facType, TypeFactoryReflection.TableFieldName.TypeDict)().AlwaysAdd(uId, ei.entityType);
                    }
                    catch (Exception ex)
                    {

                        log.LogError(ex);
                    }
                }
            }
        }

        internal static void UnRegisterTypes(Type facType, UserInfo user) 
        {

            var hasTypes = user.entityInfos.TryGetValue(facType, out List<EntityInfo> typesToRegister);
            if (hasTypes)
            {
                foreach (var ei in typesToRegister)
                {

                    if (UniqueTracker.Instance.invalidRegistrations.Contains(ei.definitionType) || !user.definitionInfos.ContainsKey(ei.definitionType))
                        continue;

                    var definition = user.definitionInfos[ei.definitionType];
                    var uId = definition.UniqueId;

                    if (!user.IsForOverwriting(ei.definitionType))
                    {
                        TypeFactoryReflection.GetAccessRef(facType, TypeFactoryReflection.TableFieldName.FullNameTypeDict)().Remove(ei.entityType.FullName);

                        TypeFactoryReflection.GetAccessRef(facType, TypeFactoryReflection.TableFieldName.TypeDict)().Remove(uId);

                    }
                    else
                    {

                        var originalType = user.typeName2VanillaType[uId];
                        // restore original types
                        TypeFactoryReflection.GetAccessRef(facType, TypeFactoryReflection.TableFieldName.FullNameTypeDict)()[ei.entityType.FullName] = originalType;

                        TypeFactoryReflection.GetAccessRef(facType, TypeFactoryReflection.TableFieldName.TypeDict)()[uId] = originalType;



                    }



                }
            }

        }

        internal void RegisterUser(UserInfo user)
        {

            log.LogInfo($"Registering assembly: {user.assembly.GetName().Name}");


            foreach (var kv in user.definitionInfos)
            {
                var type = kv.Key;

                var entityDefinition = kv.Value;


                var validForRegistration = RegisterId(user, entityDefinition);




                if (validForRegistration)
                    if (entityDefinition is CardTemplate ct)
                    {
                        RegisterConfig(ct, user);
                    }
                    else if (entityDefinition is StatusEffectTemplate st)
                    {
                        RegisterConfig(st, user);
                    }
            }

            foreach (var kv in user.entityInfos)
            {
                RegisterTypes(kv.Key, user);
            }

        }

        // full unregistration in 
        internal void UnregisterUser(UserInfo user)
        {
            foreach (var kv in user.entityInfos)
            {
                UnRegisterTypes(kv.Key, user);
            }

        }





        internal void RegisterUsers()
        {
            foreach (var kv in sideloaderUsers.userInfos)
            {
                var user = kv.Value;

                RegisterUser(user);
            }
        }

        internal void AssetsForResourceHelper()
        {
            foreach (var kv in sideloaderUsers.userInfos)
            {
                foreach (var template in kv.Value.definitionInfos)
                {


                    var definition = template.Value;
                    if (definition is CardTemplate ct)
                    {
                        ct.Consume(ct.LoadCardImages());
                    }
                }
            }
        }


        internal void LoadLocalization()
        {
            foreach (var kv in sideloaderUsers.userInfos)
            {

                var user = kv.Value;
                user.ClearTypeToLocalize();

                foreach (var template in user.definitionInfos)
                {

                    var definition = template.Value;
                    if (definition is CardTemplate ct)
                    {
                        ct.Consume(ct.LoadText());
                    }
                }

                foreach (var kv2 in user.typesToLocalize)
                {
                    var facType = kv2.Key;
                    var locInfo = kv2.Value;

                    if (locInfo.locFiles.locTable.Empty())
                    {
                        Log.log.LogError($"{user.GUID}: no files were given for global localization option of {facType.Name}");
                        continue;
                    }

                    var termDic = locInfo.locFiles.LoadLocTable(facType, locInfo.entityLogicTypes.ToArray());

                    if (termDic != null)
                    { 
                        foreach (var term in termDic)
                        {
                            if (term.Value.Empty())
                                LocalizationFiles.MissingValueError(term.Key);
                            TypeFactoryReflection.AccessTypeLocalizers(facType)().AlwaysAdd(term.Key, term.Value);
                        }
                    }
                }

                
            }
        }



    }
}
