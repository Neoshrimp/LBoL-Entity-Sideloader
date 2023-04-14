using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Adventures;
using LBoL.Core.Attributes;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActionRecord;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Battle.Interactions;
using LBoL.Core.Cards;
using LBoL.Core.Dialogs;
using LBoL.Core.GapOptions;
using LBoL.Core.Helpers;
using LBoL.Core.Intentions;
using LBoL.Core.JadeBoxes;
using LBoL.Core.PlatformHandlers;
using LBoL.Core.Randoms;
using LBoL.Core.SaveData;
using LBoL.Core.Stations;
using LBoL.Core.Stats;
using LBoL.Core.StatusEffects;
using LBoL.Core.Units;
using LBoL.EntityLib.Adventures;
using LBoL.EntityLib.Adventures.Common;
using LBoL.EntityLib.Adventures.FirstPlace;
using LBoL.EntityLib.Adventures.Shared12;
using LBoL.EntityLib.Adventures.Shared23;
using LBoL.EntityLib.Adventures.Stage1;
using LBoL.EntityLib.Adventures.Stage2;
using LBoL.EntityLib.Adventures.Stage3;
using LBoL.EntityLib.Cards.Character.Cirno;
using LBoL.EntityLib.Cards.Character.Cirno.FairySupport;
using LBoL.EntityLib.Cards.Character.Koishi;
using LBoL.EntityLib.Cards.Character.Marisa;
using LBoL.EntityLib.Cards.Character.Reimu;
using LBoL.EntityLib.Cards.Character.Sakuya;
using LBoL.EntityLib.Cards.Devel;
using LBoL.EntityLib.Cards.Neutral;
using LBoL.EntityLib.Cards.Neutral.Black;
using LBoL.EntityLib.Cards.Neutral.Blue;
using LBoL.EntityLib.Cards.Neutral.Green;
using LBoL.EntityLib.Cards.Neutral.MultiColor;
using LBoL.EntityLib.Cards.Neutral.NoColor;
using LBoL.EntityLib.Cards.Neutral.Red;
using LBoL.EntityLib.Cards.Neutral.TwoColor;
using LBoL.EntityLib.Cards.Neutral.White;
using LBoL.EntityLib.Cards.Other.Adventure;
using LBoL.EntityLib.Cards.Other.Enemy;
using LBoL.EntityLib.Cards.Other.Misfortune;
using LBoL.EntityLib.Cards.Other.Tool;
using LBoL.EntityLib.Devel;
using LBoL.EntityLib.Dolls;
using LBoL.EntityLib.EnemyUnits.Character;
using LBoL.EntityLib.EnemyUnits.Character.DreamServants;
using LBoL.EntityLib.EnemyUnits.Lore;
using LBoL.EntityLib.EnemyUnits.Normal;
using LBoL.EntityLib.EnemyUnits.Normal.Bats;
using LBoL.EntityLib.EnemyUnits.Normal.Drones;
using LBoL.EntityLib.EnemyUnits.Normal.Guihuos;
using LBoL.EntityLib.EnemyUnits.Normal.Maoyus;
using LBoL.EntityLib.EnemyUnits.Normal.Ravens;
using LBoL.EntityLib.EnemyUnits.Opponent;
using LBoL.EntityLib.Exhibits;
using LBoL.EntityLib.Exhibits.Adventure;
using LBoL.EntityLib.Exhibits.Common;
using LBoL.EntityLib.Exhibits.Mythic;
using LBoL.EntityLib.Exhibits.Seija;
using LBoL.EntityLib.Exhibits.Shining;
using LBoL.EntityLib.JadeBoxes;
using LBoL.EntityLib.Mixins;
using LBoL.EntityLib.PlayerUnits;
using LBoL.EntityLib.Stages;
using LBoL.EntityLib.Stages.NormalStages;
using LBoL.EntityLib.StatusEffects.Basic;
using LBoL.EntityLib.StatusEffects.Cirno;
using LBoL.EntityLib.StatusEffects.Enemy;
using LBoL.EntityLib.StatusEffects.Enemy.SeijaItems;
using LBoL.EntityLib.StatusEffects.Marisa;
using LBoL.EntityLib.StatusEffects.Neutral;
using LBoL.EntityLib.StatusEffects.Neutral.Black;
using LBoL.EntityLib.StatusEffects.Neutral.Blue;
using LBoL.EntityLib.StatusEffects.Neutral.Green;
using LBoL.EntityLib.StatusEffects.Neutral.Red;
using LBoL.EntityLib.StatusEffects.Neutral.TwoColor;
using LBoL.EntityLib.StatusEffects.Neutral.White;
using LBoL.EntityLib.StatusEffects.Others;
using LBoL.EntityLib.StatusEffects.Reimu;
using LBoL.EntityLib.StatusEffects.Sakuya;
using LBoL.EntityLib.UltimateSkills;
using LBoL.Presentation;
using LBoL.Presentation.Animations;
using LBoL.Presentation.Bullet;
using LBoL.Presentation.Effect;
using LBoL.Presentation.I10N;
using LBoL.Presentation.UI;
using LBoL.Presentation.UI.Dialogs;
using LBoL.Presentation.UI.ExtraWidgets;
using LBoL.Presentation.UI.Panels;
using LBoL.Presentation.UI.Transitions;
using LBoL.Presentation.UI.Widgets;
using LBoL.Presentation.Units;
using LBoLEntitySideloader.Attributes;
using LBoLEntitySideloader.Entities;
using LBoLEntitySideloader.ReflectionHelpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;
using Untitled;
using Untitled.ConfigDataBuilder;
using Untitled.ConfigDataBuilder.Base;
using Debug = UnityEngine.Debug;

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


            var exportedTypes = assembly.GetExportedTypes();

            userInfo.assembly = assembly;

            var foundEntityTypesDefinitionTypes = new HashSet<Type>();

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

                    if (bepinDependencies is null)
                    {
                        log.LogWarning($"{assembly.GetName().Name}: {type} does not have a {typeof(BepInDependency).Name} attribute");
                    }
                    else
                    {
                        bool depFound = false;
                        foreach (var bd in bepinDependencies)
                        {
                            if (bd.DependencyGUID == PluginInfo.GUID && bd.Flags == BepInDependency.DependencyFlags.HardDependency)
                            {
                                depFound = true;
                                break;
                            }
                        }
                        if (!depFound)
                        {
                            log.LogWarning($"{assembly.GetName().Name}: {type} does not have a {typeof(BepInDependency).Name} attribute with {PluginInfo.GUID} as hard dependency");
                        }
                    }
                }


                // final templates need to be Sealed
                if (type.IsSealed)
                {
                    // 2do maybe more error feedback
                    // 2do check for errors only in dev mode
                    if (type.IsSubclassOf(typeof(EntityDefinition)))
                    {

                        var definition = (EntityDefinition)Activator.CreateInstance(type);

                        userInfo.definitionInfos.Add(type, definition);

                        definition.assembly = userInfo.assembly;


                        var overwritte = type.SingularAttribute<OverwriteVanilla>();
                        

                        // 2do add optional DontLoad attribute filter
                        if (overwritte != null)
                        {
                            throw new NotImplementedException();
                        }
                    }
                    else

                    {
                        var facType = TypeFactoryReflection.factoryTypes.FirstOrDefault(t => type.IsSubclassOf(t));
                        if (facType != null)
                        {
                            userInfo.entityInfos.TryAdd(facType, new List<EntityInfo>());

                            var entityLogic = type.SingularAttribute<EntityLogic>();

                            if (entityLogic is null)
                            {
                                log.LogError($"{assembly.GetName().Name}: {type.Name} does not have {typeof(EntityLogic).Name} attribute despite having qualities of an entity type. Please add {typeof(EntityLogic).Name} attribute.");
                            }
                            else
                            {
                                if (foundEntityTypesDefinitionTypes.Contains(entityLogic.DefinitionType))
                                {
                                    log.LogError($"{assembly.GetName().Name}: {entityLogic.DefinitionType} already has an entity type associated. Entity can only have type defining its logic. Please remove {typeof(EntityLogic).Name} attribute.");
                                }
                                else
                                {
                                    foundEntityTypesDefinitionTypes.Add(entityLogic.DefinitionType);

                                    var entityInfo = new EntityInfo(facType, type, entityLogic.DefinitionType);
                                    userInfo.entityInfos[facType].Add(entityInfo);

                                    userInfo.definition2EntityType.Add(entityLogic.DefinitionType, entityInfo.entityType);
                                }

                            }

                            // 2do errors: mismatched entitylogic and entity types expected by definition

                        }
                    }
                }
            }

            log.LogInfo($"{assembly.GetName().Name} scanned!");

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
                userInfos.Add(assembly, ScanAssembly(assembly));
            }

            public void UnregisterUser(Assembly assembly)
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
            }



            public Type GetEntityLogicType(Assembly assembly, Type definitionType)
            {
                if (this.userInfos.TryGetValue(assembly, out UserInfo user) && user.definition2EntityType.TryGetValue(definitionType, out Type entityType))
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
            try
            {
                UniqueIdTracker.AddUniqueId(entityDefinition, user);
            }
            catch (Exception ex)
            {

                log.LogError(ex);
                return false;
            }


            return true;

        }


        internal void RegisterConfig<C>(IConfigProvider<C> configProvider, EntityDefinition entityDefinition = null) where C : class
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
                


                var f_Index = ConfigReflection.HasIndex(configType);

                if (f_Index != null)
                {
                    f_Index.SetValue(newConfig, UniqueIdTracker.AddUniqueIndex(IdContainer.CastFromObject(f_Index.GetValue(newConfig)), entityDefinition));
                }



                log.LogInfo($"Registering config: {f_Id.GetValue(newConfig)},  type:{entityDefinition.ConfigType().Name}");

                //var m_FromId = ConfigReflection.GetFromIdMethod(configType);

                //var config = m_FromId.Invoke(null, new object[] { Id});

                /*                if (config == null)
                                {*/

                // Add config to array
                var f_Data = ConfigReflection.GetArrayField(configType);
                // cache maybe
                var ref_Data = AccessTools.StaticFieldRefAccess<C[]>(f_Data);
                ref_Data() = ref_Data().AddToArray(newConfig).ToArray();
                // Add config to dictionary
                var f_IdTable = ConfigReflection.GetTableField(configType);
                ((Dictionary<string, C>)f_IdTable.GetValue(null)).Add(entityDefinition.UniqueId, newConfig);

/*                }
                else
                {
                    log.LogInfo($"secondary config reload for {Id}");
                    config = newConfig;
                }*/

            }
            catch (Exception ex)
            {
                log.LogError($"Exception registering {entityDefinition}: {ex}");
            }
        }

       


        internal static void RegisterTypes(Type facType, UserInfo user)
        {
           
            var hasTypes = user.entityInfos.TryGetValue(facType, out List<EntityInfo> typesToRegister);

            if (hasTypes)
            {
                foreach (var ei in typesToRegister)
                {
                    log.LogDebug($"TypeFactory<{facType.Name}>, id: {ei.entityType.Name} from {user.assembly.GetName().Name}");

                    if (!TypeFactoryReflection.GetAccessRef(facType, TypeFactoryReflection.TableFieldName.FullNameTypeDict)().TryAdd(ei.entityType.FullName, ei.entityType))
                    {
                        log.LogError($"RegisterType: {ei.entityType.Name} matches an already registered type. Please change plugin namespace.");
                    }

                    // should be type.name for now
                    var uId = UniqueIdTracker.GetUniqueId(user.definitionInfos[ei.definitionType]);

                    if(uId != ei.entityType.Name )
                    {

                        log.LogError($"{user.GUID} entity id, {uId}, mismatches entity type name, {ei.entityType.Name}");
                    }

                    TypeFactoryReflection.GetAccessRef(facType, TypeFactoryReflection.TableFieldName.TypeDict)().TryAdd(uId, ei.entityType);
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

                    TypeFactoryReflection.GetAccessRef(facType, TypeFactoryReflection.TableFieldName.FullNameTypeDict)().Remove(ei.entityType.FullName);

                    var uId = UniqueIdTracker.GetUniqueId(user.definitionInfos[ei.definitionType]);

                    TypeFactoryReflection.GetAccessRef(facType, TypeFactoryReflection.TableFieldName.TypeDict)().Remove(uId);
                }
            }

        }

        internal void RegisterUser(UserInfo user)
        {


            foreach (var kv in user.definitionInfos)
            {
                var type = kv.Key;

                var entityDefinition = kv.Value;



                if (!RegisterId(user, entityDefinition))
                    continue;

                if (entityDefinition is CardTemplate ct)
                {
                    RegisterConfig(ct);
                }
                else if (entityDefinition is StatusEffectTemplate st)
                {
                    RegisterConfig(st);
                }
            }

            foreach (var kv in user.entityInfos)
            {
                RegisterTypes(kv.Key, user);
            }

        }

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
                log.LogDebug(user.assembly.GetName().Name);

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
                foreach (var template in kv.Value.definitionInfos)
                {

                    var definition = template.Value;
                    if (definition is CardTemplate ct)
                    {
                        ct.Consume(ct.LoadYaml());
                    }
                }
            }
        }



    }
}
