using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using LBoL.Base.Extensions;
using LBoL.Core;
using LBoL.Core.Adventures;
using LBoLEntitySideloader.Attributes;
using LBoLEntitySideloader.Entities;
using LBoLEntitySideloader.ReflectionHelpers;
using LBoLEntitySideloader.Resource;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Reflection;

namespace LBoLEntitySideloader
{
    public partial class EntityManager
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

        public HashSet<Assembly> loadedFromDisk = new HashSet<Assembly>();

        public static UserInfo ScanAssembly(Assembly assembly)
        {

            var userInfo = new UserInfo();
            userInfo.assembly = assembly;

            if (BepinexPlugin.devModeConfig.Value && !string.IsNullOrEmpty(assembly.Location))
            {
                Instance.loadedFromDisk.Add(assembly);
            }

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


                        if (overwrite != null)
                        {
                            userInfo.entitiesToOverwrite.Add(type, new ModificationInfo() { attribute = overwrite });
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
                            Log.LogDevExtra()?.LogWarning($"(Extra logging) {assembly.GetName().Name}: {type.Name} does not have {typeof(EntityLogic).Name} attribute despite having qualities of an entity logic type. Please add {typeof(EntityLogic).Name} attribute.");
                        }
                        else
                        {
                            if (foundEntityLogicForDefinitionTypes.Contains(entityLogic.DefinitionType))
                            {
                                log.LogError($"{assembly.GetName().Name}: {entityLogic.DefinitionType} already has an entity logic type associated. Entity can only have one type defining its logic. Please remove {typeof(EntityLogic).Name} attribute.");
                            }
                            else if (BepinexPlugin.devModeConfig.Value && !TemplatesReflection.IsTemplateType(entityLogic.DefinitionType))
                            {
                                log.LogError($"{entityLogic.DefinitionType.Name} type provided to {typeof(EntityLogic).Name} attribute on {type.Name} is not an {typeof(EntityDefinition).Name}. Entity definition must extend one of the entity templates.");
                            }
                            else
                            {

                                foundEntityLogicForDefinitionTypes.Add(entityLogic.DefinitionType);

                                var entityInfo = new EntityInfo(facType, type, entityLogic.DefinitionType);
                                userInfo.entityInfos[facType].Add(entityInfo);

                                userInfo.definition2customEntityLogicType.Add(entityLogic.DefinitionType, entityInfo.entityType);
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
                    if (userInfo.definition2customEntityLogicType.TryGetValue(ed, out Type entityLogicType) && userInfo.definitionInfos.TryGetValue(ed, out EntityDefinition definition))

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


        internal C RegisterConfig<C>(IConfigProvider<C> configProvider, UserInfo user, EntityDefinition entityDefinition = null) where C : class
        {

            if (entityDefinition == null)
            {
                entityDefinition = (EntityDefinition)configProvider;
            }


            try
            {
                var configType = entityDefinition.ConfigType();


                var f_Id = ConfigReflection.GetIdField(configType);



                Log.LogDev()?.LogDebug($"Registering config: id: {entityDefinition.UniqueId}, config type:{entityDefinition.ConfigType().Name}");



                // For adding config to array
                var f_Data = ConfigReflection.GetArrayField(configType);

                var ref_Data = AccessTools.StaticFieldRefAccess<C[]>(f_Data);

                // For adding config to dictionary
                var f_IdTable = ConfigReflection.GetTableField(configType);



                if (!user.IsForOverwriting(entityDefinition.GetType()))
                {

                    var newConfig = configProvider.MakeConfig();
                    if (newConfig == null)
                        throw new ArgumentException($"{nameof(configProvider.MakeConfig)} must return a non-null value.");

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


                    var f_Index = ConfigReflection.HasIndex(configType);
                    if (f_Index != null)
                    {
                        f_Index.SetValue(newConfig, UniqueTracker.AddUniqueIndex(IdContainer.CastFromObject(f_Index.GetValue(newConfig)), entityDefinition));
                    }

                    ((Dictionary<string, C>)f_IdTable.GetValue(null)).Add(entityDefinition.UniqueId, newConfig);
                    ref_Data() = ref_Data().AddToArray(newConfig).ToArray();

                    return newConfig;
                }
                else
                {
                    var newConfig = configProvider.MakeConfig();
                    if (newConfig == null)
                        throw new ArgumentException($"{nameof(configProvider.MakeConfig)} must return a non-null value.");
                    HandleOverwriteWrap(() =>
                    {
                       
                        var i = UniqueTracker.Instance.id2ConfigListIndex[configType][IdContainer.CastFromObject(f_Id.GetValue(newConfig))];
                        ((Dictionary<string, C>)f_IdTable.GetValue(null)).AlwaysAdd(entityDefinition.UniqueId, newConfig);
                        ref_Data()[i] = newConfig;
                    }, entityDefinition, nameof(configProvider.MakeConfig), user);
                    return newConfig;
                }



            }
            catch (Exception ex)
            {
                log.LogError($"Exception registering config of {entityDefinition}: {ex}");
                return null;
            }
        }



        internal static void RegisterTypes(Type facType, UserInfo user)
        {

            var hasTypes = user.entityInfos.TryGetValue(facType, out List<EntityInfo> typesToRegister);

            if (hasTypes)
            {
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

                            if (!TypeFactoryReflection.AccessTypeDicts(facType, TypeFactoryReflection.TableFieldName.FullNameTypeDict)().TryAdd(ei.entityType.FullName, ei.entityType))
                            {
                                log.LogError($"RegisterType: {ei.entityType.Name} matches an already registered type. Please change plugin namespace.");
                            }
                        }
                        else
                        {

                            var id = definition.GetId();

                            if (!UniqueTracker.IsOverwriten(facType, id, "EntityLogic", ei.definitionType, user))
                            {
                                var originalType = TypeFactoryReflection.AccessTypeDicts(facType, TypeFactoryReflection.TableFieldName.TypeDict)()[uId];

                                user.typeName2VanillaType.Add(id, originalType);

                                TypeFactoryReflection.AccessTypeDicts(facType, TypeFactoryReflection.TableFieldName.FullNameTypeDict)()[originalType.FullName] = ei.entityType;
                            }



                        }



                        if (!user.IsForOverwriting(ei.definitionType))
                            TypeFactoryReflection.AccessTypeDicts(facType, TypeFactoryReflection.TableFieldName.TypeDict)().Add(uId, ei.entityType);
                        else
                            TypeFactoryReflection.AccessTypeDicts(facType, TypeFactoryReflection.TableFieldName.TypeDict)().AlwaysAdd(uId, ei.entityType);


                        ProcessWeighterAttribute(facType, ei.entityType);


                    }
                    catch (Exception ex)
                    {

                        log.LogError(ex);
                    }
                }
            }
        }

        // since custom entities are loaded later than vanilla their weighter attribute never gets processed
        private static void ProcessWeighterAttribute(Type facType, Type entityType)
        {
            if (facType == typeof(Exhibit))
            {
                var exInfo = entityType.GetCustomAttribute<ExhibitInfoAttribute>();
                var weighter = exInfo?.CreateWeighter();

                if (weighter != null)
                {
                    Library._exhibitWeighterTable.AlwaysAdd(entityType, weighter);
                }
            }
            else if (facType == typeof(Adventure))
            {
                throw new NotImplementedException();
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
                        TypeFactoryReflection.AccessTypeDicts(facType, TypeFactoryReflection.TableFieldName.FullNameTypeDict)().Remove(ei.entityType.FullName);

                        TypeFactoryReflection.AccessTypeDicts(facType, TypeFactoryReflection.TableFieldName.TypeDict)().Remove(uId);

                        if (facType == typeof(Exhibit))
                        {
                            Library._exhibitWeighterTable.Remove(ei.entityType);
                        }
                        else if (facType == typeof(Adventure))
                        {
                            Library._adventureWeighterTable.Remove(ei.entityType);
                        }
                    }
                    else
                    {

                        var originalType = user.typeName2VanillaType[uId];
                        // restore original types
                        TypeFactoryReflection.AccessTypeDicts(facType, TypeFactoryReflection.TableFieldName.FullNameTypeDict)()[ei.entityType.FullName] = originalType;

                        TypeFactoryReflection.AccessTypeDicts(facType, TypeFactoryReflection.TableFieldName.TypeDict)()[uId] = originalType;

                        ProcessWeighterAttribute(facType, originalType);
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
                    else if (entityDefinition is ExhibitTemplate et)
                    {
                        RegisterConfig(et, user);
                    }
                    else if (entityDefinition is BgmTemplate bt)
                    {
                        var bgmConfig = RegisterConfig(bt, user);
                        // pass ID to LoadBgmAsync
                        if (bgmConfig != null)
                        {
                            bgmConfig.Path = bgmConfig.ID;
                        }
                    }

            }


            Log.LogDev()?.LogInfo($"Registering entity logic types from assembly: {user.assembly.GetName().Name}"); 
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

            log.LogInfo($"All sideloader users registered!");
        }

        internal void LoadAssetsForResourceHelper()
        {
            foreach (var kv in sideloaderUsers.userInfos)
            {
                var user = kv.Value;
                foreach (var kv2 in kv.Value.definitionInfos)
                {
                    var defType = kv2.Key;

                    var definition = kv2.Value;
                    if (definition is CardTemplate ct)
                    {
                        HandleOverwriteWrap(() => ct.Consume(ct.LoadCardImages()), definition, nameof(ct.LoadCardImages), user);
                    }
                    else if (definition is StatusEffectTemplate st)
                    {
                        HandleOverwriteWrap(() => st.Consume(st.LoadSprite()), definition, nameof(st.LoadSprite), user);
                    }
                    else if (definition is ExhibitTemplate et)
                    {
                        HandleOverwriteWrap(() => et.Consume(et.LoadSprite()), definition, nameof(et.LoadSprite), user);
                    }

                }
            }
        }



        internal void LoadLocalization()
        {
            foreach (var kv in sideloaderUsers.userInfos)
            {

                var user = kv.Value;
                user.ClearTypesToLocalize();

                foreach (var template in user.definitionInfos)
                {

                    var definition = template.Value;
                    if (definition is CardTemplate ct)
                    {
                        HandleOverwriteWrap(() => ct.Consume(ct.LoadLocalization()), definition, nameof(ct.LoadLocalization), user);

                    }
                    else if (definition is StatusEffectTemplate st)
                    {
                        HandleOverwriteWrap(() => st.Consume(st.LoadLocalization()), definition, nameof(st.LoadLocalization), user);
                    }
                    else if (definition is ExhibitTemplate et)
                    {
                        HandleOverwriteWrap(() => et.Consume(et.LoadLocalization()), definition, nameof(et.LoadLocalization), user);
                    }

                }

                // load global localization
                foreach (var kv2 in user.typesToLocalize)
                {
                    var facType = kv2.Key;
                    var locInfo = kv2.Value;

                    if (locInfo.locFiles == null)
                    {
                        Log.log.LogError($"{user.GUID}: localization files parameter was never initialized for global localization option of {facType.Name}");
                        continue;
                    }

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


        internal void LoadAll()
        {
            try
            {
                Instance.RegisterUsers();
                Instance.LoadAssetsForResourceHelper();
                Instance.LoadLocalization();
            }
            catch (Exception e)
            {

                log.LogError(e);
            }

            log.LogInfo("Finished loading custom resources.");
        }

        static internal void HandleOverwriteWrap(Action action, EntityDefinition definition, string methodName, UserInfo user)
        {
            var defType = definition.GetType();

            if (!UniqueTracker.Instance.invalidRegistrations.Contains(defType))
            {
                if (!user.IsForOverwriting(defType))
                {
                    action();
                }
                else if (TemplatesReflection.DoOverwrite(defType, methodName) && !UniqueTracker.IsOverwriten(definition.EntityType(), definition.UniqueId, methodName, defType, user))
                {

                    action();
                }
            }

        }

    }
}
