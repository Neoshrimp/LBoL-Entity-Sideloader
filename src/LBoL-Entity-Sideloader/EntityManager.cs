using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using LBoL.Base.Extensions;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Adventures;
using LBoLEntitySideloader.Attributes;
using LBoLEntitySideloader.Entities;
using LBoLEntitySideloader.ReflectionHelpers;
using LBoLEntitySideloader.Resource;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Reflection;
using System.Threading;
using UnityEngine.Events;
using YamlDotNet.RepresentationModel;

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

        public HashSet<Assembly> loadedFromDiskUsers = new HashSet<Assembly>();

        public HashSet<Action> loadedFromDiskPostAction = new HashSet<Action>();

        public SideloaderUsers sideloaderUsers = new SideloaderUsers();

        public SideloaderUsers secondaryUsers = new SideloaderUsers();

        public List<Action> loadedFromDiskCharLoadouts = new List<Action>();

        public List<Func<List<Stage>, List<Stage>>> loadedFromDiskmodifyStageListFuncs = new List<Func<List<Stage>, List<Stage>>>();

        public List<UniqueTracker.StageModAction> loadedFromModifyStageActions = new List<UniqueTracker.StageModAction>();


        public static UserInfo ScanAssembly(Assembly assembly, bool lookForFactypes = true)
        {

            var userInfo = new UserInfo();
            userInfo.assembly = assembly;

            if (!assembly.IsDynamic && BepinexPlugin.devModeConfig.Value && !string.IsNullOrEmpty(assembly.Location))
            {
                Instance.loadedFromDiskUsers.Add(assembly);
            }

            Log.LogDev()?.LogInfo($"Scanning {assembly.GetName().Name}...");

            var exportedTypes = !assembly.IsDynamic ? assembly.GetExportedTypes() : assembly.GetTypes();

            userInfo.assembly = assembly;

            var foundEntityLogicForDefinitionTypes = new HashSet<Type>();

            foreach (var type in exportedTypes)
            {

                if (!assembly.IsDynamic && type.IsSubclassOf(typeof(BaseUnityPlugin)))
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

                        definition.userAssembly = userInfo.assembly;
                        definition.user = userInfo;


                        var overwrite = type.GetCustomAttribute<OverwriteVanilla>(true);


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


                        if (type.GetCustomAttribute<EntityLogic>() is EntityLogic entityLogicAtt)
                        {
                            if (foundEntityLogicForDefinitionTypes.Contains(entityLogicAtt.DefinitionType))
                            {
                                log.LogError($"{assembly.GetName().Name}: {entityLogicAtt.DefinitionType} already has an entity logic type associated. Entity can only have one type defining its logic. Please remove {typeof(EntityLogic).Name} attribute.");
                            }
                            else if (BepinexPlugin.devModeConfig.Value && !TemplatesReflection.IsTemplateType(entityLogicAtt.DefinitionType))
                            {
                                log.LogError($"{entityLogicAtt.DefinitionType.Name} type provided to {typeof(EntityLogic).Name} attribute on {type.Name} is not an {typeof(EntityDefinition).Name}. Entity definition must extend one of the entity templates.");
                            }
                            else
                            {

                                foundEntityLogicForDefinitionTypes.Add(entityLogicAtt.DefinitionType);

                                var entityInfo = new EntityInfo(facType, type, entityLogicAtt.DefinitionType);
                                userInfo.entityInfos[facType].Add(entityInfo);

                                userInfo.definition2customEntityLogicType.Add(entityLogicAtt.DefinitionType, entityInfo.entityType);
                            }

                        }
                        else if (type.GetCustomAttribute<ExternalEntityLogicAttribute>() is ExternalEntityLogicAttribute externalEntityLogicAtt)
                        {
                            // DEEZNUTS
                        }
                        else
                        {
                            Log.LogDevExtra()?.LogWarning($"(Extra logging) {assembly.GetName().Name}: {type.Name} does not have {typeof(EntityLogic).Name} attribute despite having qualities of an entity logic type. Please add {typeof(EntityLogic).Name} attribute.");
                        }
                    }
                    else if (BepinexPlugin.devModeConfig.Value && !type.IsSealed)
                    {
                        Log.LogDevExtra()?.LogWarning($"(Extra logging) {assembly.GetName().Name}: {type} is subtype of {facType.Name} but isn't sealed. Final entity logic types need to be sealed");
                    }
                    continue;
                }
            }


            if (BepinexPlugin.devModeConfig.Value && BepinexPlugin.devExtraLoggingConfig.Value && !assembly.IsDynamic)
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
                        log.LogWarning($"(Extra logging) {defType.Name} needs entity logic type extending {defVal.EntityType().Name} but none was found. Did you define public sealed entity logic class with {typeof(EntityLogic).Name} attribute?");
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



        public static void AddExternalDefinitionTypePromise(Type entityLogicType, Func<Type> defTypePromise, Assembly userAssembly = null)
        {
            if(userAssembly  == null)
                userAssembly = Assembly.GetCallingAssembly();



            UniqueTracker.Instance.typePromiseDic.TryAdd(userAssembly, new Dictionary<Type, List<UniqueTracker.DefTypePromisePair>>());

            var facType = TypeFactoryReflection.factoryTypes.FirstOrDefault(t => entityLogicType.IsSubclassOf(t));

            if (facType == null)
            {
                throw new ArgumentException($"{entityLogicType} does not inherit from general entity logic types");
            }

            UniqueTracker.Instance.typePromiseDic[userAssembly].TryAdd(facType, new List<UniqueTracker.DefTypePromisePair>());

            UniqueTracker.Instance.typePromiseDic[userAssembly][facType].Add(new UniqueTracker.DefTypePromisePair() { entityLogicType = entityLogicType, defTypePromise = defTypePromise });

        }
        
        
        /// <summary>
        /// Add an action which will be executed after all main entities were loaded.
        /// Should be called from BePinEx Awake injection point.
        /// Currently used for template gen.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="callingAssembly"></param>
        public static void AddPostLoadAction(Action action, Assembly callingAssembly = null)
        {

            if (callingAssembly == null)
                callingAssembly = Assembly.GetCallingAssembly();


            Action postLoadAction = () => {
                try
                {
                    action();
                }
                catch (Exception ex)
                {
                    log.LogError($"Error during template generation: {ex}");
                }
            };

            if (!callingAssembly.IsDynamic && !string.IsNullOrEmpty(callingAssembly.Location))
                EntityManager.Instance.loadedFromDiskPostAction.Add(postLoadAction);

            UniqueTracker.Instance.PostMainLoad += postLoadAction;
        }




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

            Log.LogDevExtra()?.LogDebug($"(Extra logging) Registering id:  template: {entityDefinition.GetType().Name}, id: {entityDefinition.GetId()}, IsForOverwriting: {user.IsForOverwriting(entityDefinition.GetType())}");

            var templateType = entityDefinition.GetType();
            try
            {
                if (user.entitiesToOverwrite.ContainsKey(templateType))
                {
                    if (!UniqueTracker.Instance.id2ConfigListIndex[entityDefinition.ConfigType()].ContainsKey(entityDefinition.GetId()))

                    {
                        log.LogError($"RegisterId: {entityDefinition.GetId()} was not found among vanilla ids. Overwriting is not supported for non-vanilla entities (for now, maybe).");
                        UniqueTracker.Instance.invalidRegistrations.Add(templateType);
                        return false;
                    }
                    return true;
                }

                UniqueTracker.AddUniqueId(entityDefinition, user);
            }
            catch (Exception ex)
            {

                log.LogError(ex);
                UniqueTracker.Instance.invalidRegistrations.Add(templateType);
                return false;
            }


            return true;

        }



        internal void RegisterUser(UserInfo user)
        {
            
            log.LogInfo($"Registering assembly: {user.assembly.GetName().Name}");
            Log.LogDev()?.LogDebug($"Adding configs..");

            var stopwatch = new System.Diagnostics.Stopwatch();
            if (BepinexPlugin.devModeConfig.Value)
                stopwatch.Start();

            foreach (var kv in user.definitionInfos)
            {
                var type = kv.Key;

                var entityDefinition = kv.Value;


                var validForRegistration = RegisterId(user, entityDefinition);

                if (validForRegistration)
                {
                    try
                    {
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

                            // should only be the case if template is for overwrite but config was NOT overwritten. Alternative:
                            // if (user.IsForOverwriting(bt.GetType()) && !TemplatesReflection.DoOverwrite(bt.GetType(), nameof(BgmTemplate.MakeConfig)))
                            if (bgmConfig == null)
                            {
                                bgmConfig = BgmConfig.FromID(bt.UniqueId);
                            }

                            // in case config is overwritten but not the bgm (dont make sense really)
                            if (!user.IsForOverwriting(bt.GetType()) || user.IsForOverwriting(bt.GetType()) && TemplatesReflection.DoOverwrite(bt.GetType(), nameof(BgmTemplate.LoadAudioClipAsync)))
                            {
                                // pass ID to LoadBgmAsync
                                bgmConfig.Path = bgmConfig.ID;
                                bgmConfig.Folder = "";
                            }
                            UniqueTracker.Instance.AddOnDemandResource(entityDefinition.TemplateType(), bgmConfig.ID, entityDefinition);


                        }
                        else if (entityDefinition is SfxTemplate sfxT)
                        {
                            RegisterConfig(sfxT, user);
                        }
                        else if (entityDefinition is UiSoundTemplate usfxT)
                        {
                            RegisterConfig(usfxT, user);
                        }
                        else if (entityDefinition is JadeBoxTemplate jt)
                        {
                            RegisterConfig(jt, user);
                        }
                        else if (entityDefinition is EnemyUnitTemplate eut)
                        {
                            RegisterConfig(eut, user);
                        }
                        else if (entityDefinition is UltimateSkillTemplate ust)
                        {
                            RegisterConfig(ust, user);
                        }
                        else if (entityDefinition is EnemyGroupTemplate egT)
                        {
                            RegisterConfig(egT, user);
                        }
                        else if (entityDefinition is PlayerUnitTemplate puT)
                        {
                            var puTConfig = RegisterConfig(puT, user);
                            UniqueTracker.Instance.AddOnDemandResource(puT.TemplateType(), puTConfig.Id, puT);
                        }
                        else if (entityDefinition is UnitModelTemplate umT)
                        {
                            var umTConfig = RegisterConfig(umT, user);
                            UniqueTracker.Instance.AddOnDemandResource(umT.TemplateType(), umTConfig.Name, umT);
                        }
                        else if (entityDefinition is StageTemplate scT)
                        {
                            RegisterConfig(scT, user);
                        }
                    }
                    catch (Exception ex)
                    {
                        log.LogError($"Exception registering config of {entityDefinition}: {ex}");
                    }
                }

            }


            Log.LogDev()?.LogDebug($"Adding entity logic types..");
            foreach (var kv in user.entityInfos)
            {
                RegisterTypes(kv.Key, user);
            }

            var msg = $"Registering assembly {user.assembly.GetName().Name} done!";
            if (BepinexPlugin.devModeConfig.Value)
            {
                stopwatch.Stop();
                msg += $" Elapsed time: {stopwatch.ElapsedMilliseconds}ms";
            }


            log.LogInfo(msg);
        }

        internal C RegisterConfig<C>(IConfigProvider<C> configProvider, UserInfo user, EntityDefinition entityDefinition = null) where C : class
        {

            if (entityDefinition == null)
            {
                entityDefinition = (EntityDefinition)configProvider;
            }


            var configType = entityDefinition.ConfigType();
            var defType = entityDefinition.GetType();


            var f_Id = ConfigReflection.GetIdField(configType);



            Log.LogDevExtra()?.LogDebug($"(Extra Logging) Registering config: id: {entityDefinition.UniqueId}, config type:{entityDefinition.ConfigType().Name}");



            // For adding config to array
            var f_Data = ConfigReflection.GetArrayField(configType);

            var ref_Data = AccessTools.StaticFieldRefAccess<C[]>(f_Data);

            // For adding config to dictionary
            var f_IdTable = ConfigReflection.GetTableField(configType);

            C newConfig = null;

            if (!UniqueTracker.Instance.invalidRegistrations.Contains(defType) && (!user.IsForOverwriting(entityDefinition.GetType()) || TemplatesReflection.DoOverwrite(defType, nameof(configProvider.MakeConfig))))
            {
                newConfig = configProvider.MakeConfig();
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
            }



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

                if (!UniqueTracker.Instance.invalidRegistrations.Contains(defType))
                {
                    if (TemplatesReflection.DoOverwrite(defType, nameof(configProvider.MakeConfig)) && !UniqueTracker.IsOverwriten(entityDefinition.TemplateType(), entityDefinition.UniqueId, nameof(configProvider.MakeConfig), defType, user))
                    {


                        var i = UniqueTracker.Instance.id2ConfigListIndex[configType][IdContainer.CastFromObject(f_Id.GetValue(newConfig))];

                        ((Dictionary<string, C>)f_IdTable.GetValue(null)).AlwaysAdd(entityDefinition.UniqueId, newConfig);
                        ref_Data()[i] = newConfig;
                    }
                }

            }


            return newConfig;


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
                        Log.LogDevExtra()?.LogDebug($"(Extra Logging) Registering entity logic type in TypeFactory<{facType.Name}>, typeName: {ei.entityType.Name}, from template: {ei.definitionType.Name}");

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

                            if (!UniqueTracker.IsOverwriten(definition.TemplateType(), id, "EntityLogic", ei.definitionType, user))
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



        // full unregistration in 
        internal void UnregisterUser(UserInfo user)
        {
            foreach (var kv in user.entityInfos)
            {
                UnRegisterTypes(kv.Key, user);
            }

        }



        internal void RegisterUsers(SideloaderUsers sideloaderUsers)
        {
            foreach (var kv in sideloaderUsers.userInfos)
            {
                var user = kv.Value;

                RegisterUser(user);
            }

            log.LogInfo($"All sideloader users registered!");
        }

        internal void LoadAssetsForResourceHelper(SideloaderUsers sideloaderUsers)
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
                    else if (definition is UltimateSkillTemplate ust)
                    {
                        HandleOverwriteWrap(() => ust.Consume(ust.LoadSprite()), definition, nameof(ust.LoadSprite), user);
                    }
                    else if (definition is SfxTemplate sfxT)
                    {
                        HandleOverwriteWrap(() => sfxT.Consume(sfxT.LoadSfxListAsync()), definition, nameof(sfxT.LoadSfxListAsync), user);
                    }
                    else if (definition is UiSoundTemplate usfxT)
                    {
                        HandleOverwriteWrap(() => usfxT.Consume(usfxT.LoadSfxListAsync()), definition, nameof(usfxT.LoadSfxListAsync), user);
                    }
                    else if (definition is PlayerUnitTemplate puT)
                    {
                        // overwrite handled later
                        puT.Consume(puT.LoadPlayerImages());
                    }
                    


                }
            }
        }



        internal void LoadLocalization(SideloaderUsers sideloaderUsers)
        {
            foreach (var kv in sideloaderUsers.userInfos)
            {

                var user = kv.Value;

                UniqueTracker.Instance.typesToLocalize[user.assembly] = new Dictionary<Type, LocalizationInfo>();
                //user.ClearTypesToLocalize();

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
                    else if (definition is JadeBoxTemplate jt)
                    {
                        HandleOverwriteWrap(() => jt.Consume(jt.LoadLocalization()), definition, nameof(jt.LoadLocalization), user);
                    }
                    else if (definition is EnemyUnitTemplate eut)
                    {
                        HandleOverwriteWrap(() => eut.Consume(eut.LoadLocalization()), definition, nameof(eut.LoadLocalization), user);
                    }
                    else if (definition is UltimateSkillTemplate ust)
                    {
                        HandleOverwriteWrap(() => ust.Consume(ust.LoadLocalization()), definition, nameof(ust.LoadLocalization), user);
                    }
                    else if (definition is UnitModelTemplate umT)
                    {
                        HandleOverwriteWrap(() => umT.Consume(umT.LoadLocalization()), definition, nameof(umT.LoadLocalization), user);
                    }
                    else if (definition is PlayerUnitTemplate puT)
                    {
                        HandleOverwriteWrap(() => puT.Consume(puT.LoadLocalization()), definition, nameof(puT.LoadLocalization), user);
                    }
                }

                // load global localization
                foreach (var kv2 in UniqueTracker.Instance.typesToLocalize[user.assembly])
                {
                    var facType = kv2.Key;
                    var locInfo = kv2.Value;

                    if (locInfo.locFiles == null)
                    {
                        Log.log.LogError($"{user.assembly.GetName().Name}: localization files parameter was never initialized for global localization option of {facType.Name}");
                        continue;
                    }

                    if (locInfo.locFiles.locTable.Empty())
                    {
                        Log.log.LogError($"{user.GUID}: no files were given for global localization option of {facType.Name}");
                        continue;
                    }

                    var termDic = locInfo.locFiles.LoadLocTable(facType, locInfo.entityLogicTypes.ToArray());

                    // 2do? one merge terms parameter per global localization
                    LocalizationOption.FillLocalizationTables(termDic, facType, locInfo.locFiles.mergeTerms);

                }

                if (UniqueTracker.Instance.unitNamesGlobalLocalizationFiles.TryGetValue(user.assembly, out var locfiles))
                {
                    LocalizationOption.FillUnitNameTable(locfiles.Load(Localization.CurrentLocale), locfiles.mergeTerms, UniqueTracker.Instance.unitIdsToLocalize[user.assembly]);
                }

            }
        }


        internal void LoadAll(SideloaderUsers sideloaderUsers, bool loadLoc = true)
        {
            try
            {
                Instance.RegisterUsers(sideloaderUsers);
                Instance.LoadAssetsForResourceHelper(sideloaderUsers);
                if(loadLoc)
                    Instance.LoadLocalization(sideloaderUsers);
            }
            catch (Exception ex)
            {

                log.LogError(ex);
            }

            log.LogInfo("Finished loading custom resources.");
        }

        static internal bool HandleOverwriteWrap(Action action, EntityDefinition definition, string methodName, UserInfo user)
        {
            try
            {
                var defType = definition.GetType();

                if (!UniqueTracker.Instance.invalidRegistrations.Contains(defType))
                {
                    if (!user.IsForOverwriting(defType))
                    {
                        action();
                        return true;
                    }
                    else if (TemplatesReflection.DoOverwrite(defType, methodName) && !UniqueTracker.IsOverwriten(definition.TemplateType(), definition.UniqueId, methodName, defType, user))
                    {
                        action();
                        return true;

                    }
                }
            }
            catch (Exception ex)
            {
                Log.log.LogError($"Error while processing {definition.GetType().Name}.{methodName}: {ex}");

            }
            return false;

        }

    }
}
