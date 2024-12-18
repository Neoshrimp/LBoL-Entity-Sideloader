﻿using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Cards;
using LBoL.Core.Units;
using LBoL.EntityLib.Exhibits.Shining;
using LBoL.Presentation.UI.Widgets;
using LBoLEntitySideloader.CustomHandlers;
using LBoLEntitySideloader.Entities;
using LBoLEntitySideloader.PersistentValues;
using LBoLEntitySideloader.ReflectionHelpers;
using LBoLEntitySideloader.Resource;
using LBoLEntitySideloader.TemplateGen;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace LBoLEntitySideloader
{

    // 2do index tracking relies on the fact that MakeConfig is called only once. but indexing is only for visual ordering anyway 
    public class UniqueTracker
    {
        private static readonly BepInEx.Logging.ManualLogSource log = BepinexPlugin.log;

        private static UniqueTracker _instance;


        public static UniqueTracker Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new UniqueTracker();
                    _instance.indexTable.Sequence(typeof(BgmConfig)).SetCounter(120);
                    _instance.indexTable.Sequence(typeof(PlayerUnitConfig)).SetCounter(6);
                    // 2do related to collection panel
                    _instance.indexTable.Sequence(typeof(ExhibitConfig)).SetCounter(800);


                }
                return _instance;
            }
        }

      

        internal static void DestroySelf() { _instance = null; }



        /// <summary>
        /// config type => used Ids
        /// </summary>
        public Dictionary<Type, HashSet<IdContainer>> configIds = new Dictionary<Type, HashSet<IdContainer>>();

        public Dictionary<Type, HashSet<int>> configIndexes = new Dictionary<Type, HashSet<int>>();

        /// <summary>
        /// EntityDefinition type 
        /// </summary>
        public Dictionary<Type, IdContainer> entity2uniqueIds = new Dictionary<Type, IdContainer>();

        /// <summary>
        /// configType => Id => index
        /// </summary>
        public Dictionary<Type, Dictionary<IdContainer, int>> id2ConfigListIndex = new Dictionary<Type, Dictionary<IdContainer, int>>();

        private TemplateSequenceTable tempConfigIndexTable = new TemplateSequenceTable();

        /// <summary>
        /// concrete template type
        /// </summary>
        public HashSet<Type> invalidRegistrations = new HashSet<Type>();

        /// <summary>
        /// templateType =>+ id =>+ component string =>+ OverwiteInfo(component, defType, userInfo) 
        /// </summary>
        public Dictionary<Type, Dictionary<IdContainer, Dictionary<string, OverwriteInfo>>> overwriteTracker = new Dictionary<Type, Dictionary<IdContainer, Dictionary<string, OverwriteInfo>>>();


        /// <summary>
        /// user assembly +=> generatedTemplates
        /// </summary>
        public Dictionary<Assembly, List<Assembly>> generatedAssemblies = new Dictionary<Assembly, List<Assembly>>();

        /// <summary>
        /// generated assembly +=> generating user assembly
        /// </summary>
        public Dictionary<Assembly, Assembly> gen2User = new Dictionary<Assembly, Assembly>();

        public Dictionary<Assembly, Type> gen2FacType = new Dictionary<Assembly, Type>();


        public class DefTypePromisePair
        {
            public Type entityLogicType;
            public Func<Type> defTypePromise;
        }

        /// <summary>
        ///  generating user assembly +=> facType +=> (entityLogicType, defTypePromise)
        /// </summary>
        public Dictionary<Assembly, Dictionary<Type, List<DefTypePromisePair>>> typePromiseDic = new Dictionary<Assembly, Dictionary<Type, List<DefTypePromisePair>>>();


        public Dictionary<Assembly, Dictionary<Type, LocalizationInfo>> typesToLocalize = new Dictionary<Assembly, Dictionary<Type, LocalizationInfo>>();


        public Dictionary<Assembly, Dictionary<Type, HashSet<BatchLocalization>>> batchLocalization = new Dictionary<Assembly, Dictionary<Type, HashSet<BatchLocalization>>>();


        /// <summary>
        /// user => yaml file
        /// </summary>
        public Dictionary<Assembly, LocalizationFiles> unitNamesGlobalLocalizationFiles = new Dictionary<Assembly, LocalizationFiles>();

        public Dictionary<Assembly, HashSet<IdContainer>> unitIdsToLocalize = new Dictionary<Assembly, HashSet<IdContainer>>();


        public Dictionary<Assembly, HashSet<LocalizationFiles>> spellEntriesLocFiles = new Dictionary<Assembly, HashSet<LocalizationFiles>>();

        public Dictionary<Assembly, HashSet<string>> spellIdsToLocalize = new Dictionary<Assembly, HashSet<string>>();

        public Dictionary<Assembly, Dictionary<string, SpellTemplate>> spellTemplates = new Dictionary<Assembly, Dictionary<string, SpellTemplate>>();

        public Dictionary<Assembly, Dictionary<string, UltimateSkillTemplate>> ultimateSkillTemplates = new Dictionary<Assembly, Dictionary<string, UltimateSkillTemplate>>();


        /// <summary>
        /// assembly name => method cache
        /// </summary>
        public Dictionary<string, MethodCache> methodCacheDic = new Dictionary<string, MethodCache>();

        [field: DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
        internal event Action PostMainLoad;

        public List<Action> formationAddActions = new List<Action>();

        public List<Action> environmentsAddActions = new List<Action>();

        public Dictionary<Assembly, Dictionary<string, GameObject>> createdEnvObjectCache = new Dictionary<Assembly, Dictionary<string, GameObject>>();

        /// <summary>
        /// charId +=> loadoutType +=> LoadoutInfo
        /// </summary>
        public Dictionary<string, List<CharLoadoutInfo>> loadoutInfos = new Dictionary<string, List<CharLoadoutInfo>>();

        public List<Action> populateLoadoutInfosActions = new List<Action>();

        public class CharLoadoutInfo
        {
            public string ultimateSkill;
            public string exhibit;  
            public List<string> deck;
            public int complexity;
            public string typeSuffix;
            public string typeName;
        }

        internal Dictionary<SaveDataID, CustomGameRunSaveData> customGrSaveData = new Dictionary<SaveDataID, CustomGameRunSaveData>();

        public List<Func<List<Stage>, List<Stage>>> modifyStageListFuncs = new List<Func<List<Stage>, List<Stage>>>();

        public List<StageModAction> modifyStageActions = new List<StageModAction>();

        public class StageModAction
        {
            public string Id;
            public Func<Stage, Stage> mod;
        }


        public Dictionary<Assembly, List<PlayerUnitTemplate>> user2PlayerTemplates = new Dictionary<Assembly, List<PlayerUnitTemplate>>();


        public CHandlerManager cHandlerManager = new CHandlerManager();

        private Dictionary<string, Func<Intention, string>> intentionSuffixFuncs = null;

        public Dictionary<string, Func<Intention, string>> IntentionSuffixFuncs
        {
            get
            {
                if (intentionSuffixFuncs == null)
                {
                    intentionSuffixFuncs = EntityManager.Instance.AllUsers.Select(tu => tu.userInfo)
                    .SelectMany(ui => ui.definitionInstances.Values)
                    .Where(d => d is IntentionTemplate)
                    .Cast<IntentionTemplate>()
                    .ToDictionary(it => it.UniqueId.ToString(), it => new Func<Intention, string>(it.SelectAltIconsSuffix));
                }
                return intentionSuffixFuncs;
            }
        }

        public void RaisePostMainLoad()
        {
            if (PostMainLoad == null)
            {
                Log.LogDev()?.LogInfo("No template generation was queued.");
                return;
            }

            PostMainLoad();

            generatedAssemblies.Values.ToList().ForEach(l => l.ForEach(a => EntityManager.Instance.secondaryUsers.AddUser(a)));

            foreach (var g2u in gen2User)
            {

                if(gen2FacType.TryGetValue(g2u.Key, out var facType) && typePromiseDic.TryGetValue(g2u.Value, out var promiseDic))
                    foreach (var dtpp in promiseDic[facType])
                    {
                        var defType = dtpp.defTypePromise.Invoke();

                        var entityInfo = new EntityInfo(facType, dtpp.entityLogicType, defType);

                        var userInfo = EntityManager.Instance.secondaryUsers.userInfos[g2u.Key];

                        userInfo.entityInfos.TryAdd(facType, new List<EntityInfo>());
                        userInfo.entityInfos[facType].Add(entityInfo);



                        userInfo.definition2customEntityLogicType.Add(defType, entityInfo.entityType);
                    }
            }



        }

        Sequence uIdSalt = new Sequence();

        Sequence uIntId = new Sequence(1394);


        TemplateSequenceTable indexTable = new TemplateSequenceTable(12000);

        public Dictionary<Type, int> entity2uniqueIndexes = new Dictionary<Type, int>();

        /// <summary>
        /// templateType => Id => definitionType
        /// </summary>
        public Dictionary<Type, Dictionary<string, EntityDefinition>> onDemandResourceTracker = new Dictionary<Type, Dictionary<string, EntityDefinition>>();

        
        //static private HashSet<IdContainer> uniqueIds = new HashSet<IdContainer>();

        public bool IsLoadedOnDemand(Type templateType, string Id, out EntityDefinition entityDefinition)
        {

            entityDefinition = null;
            onDemandResourceTracker.TryGetValue(templateType, out var IdDic);
            if(IdDic == null)
                return false;
            return IdDic.TryGetValue(Id, out entityDefinition);
        }


        public bool AddOnDemandResource(Type templateType, string Id, EntityDefinition definition)
        {
            onDemandResourceTracker.TryAdd(templateType, new Dictionary<string, EntityDefinition>());
            // stupid might work
            return onDemandResourceTracker[templateType].TryAdd(Id, definition);
        }

        public static bool IsOverwriten(Type templateType, IdContainer id, string component, Type definitionType, UserInfo user)
        {
            Instance.overwriteTracker.TryAdd(templateType, new Dictionary<IdContainer, Dictionary<string, OverwriteInfo>>());
            var idDic = Instance.overwriteTracker[templateType];
            if (idDic.TryGetValue(id, out Dictionary<string, OverwriteInfo> oiDic))
            {
                if (oiDic.TryGetValue(component, out OverwriteInfo oi))
                {
                    // if definition type is the same it's probably a localization change
                    if (!oi.defType.Equals(definitionType))
                    { 
                        log.LogError($"{user.assembly.GetName().Name} definition {definitionType.Name} is trying to change {component} of {id} but it's already modified by {oi.user.assembly.GetName().Name} definition {oi.defType.Name}.");
                        return true;
                    }
                    return false;
                }
                else
                {
                    oiDic.Add(component, new OverwriteInfo() { defType = definitionType, user = user });
                    return false;
                }
            }
            else
            {
                idDic.Add(id, new Dictionary<string, OverwriteInfo>());
                idDic[id].Add(component, new OverwriteInfo() { defType = definitionType, user = user });
                return false;
            }
        }


        static internal void TrackVanillaConfig(object config, bool allowDuplicateIndex = false )
        {
            Instance.configIds.TryAdd(config.GetType(), new HashSet<IdContainer>());
            var ids = Instance.configIds[config.GetType()];


            var f_id = ConfigReflection.GetIdField(config.GetType());

            IdContainer id = IdContainer.CastFromObject(f_id.GetValue(config));

            if (ids.Contains(id))
            {
                log.LogDebug(id);
                log.LogWarning($"duplicate id: {id} in {config.GetType()}");
            }
            else
            {
                ids.Add(id);
            }

            var f_index = ConfigReflection.HasIndex(config.GetType());
            if (f_index != null)
            {
                Instance.configIndexes.TryAdd(config.GetType(), new HashSet<int>());
                Instance.configIndexes.TryGetValue(config.GetType(), out HashSet<int> indexes);

                var index = (int)f_index.GetValue(config);

                if (indexes.Contains(index) && !allowDuplicateIndex)
                {
                    log.LogDebug($"index: {index}");
                    log.LogWarning($"duplicate id: {index} in {config.GetType()}");
                }
                else
                {
                    indexes.Add(index);
                }
            }

            Instance.id2ConfigListIndex.TryAdd(config.GetType(), new Dictionary<IdContainer, int>());
            // assumes this method is only being called in config loading loop
            Instance.id2ConfigListIndex[config.GetType()].Add(id, Instance.tempConfigIndexTable.Next(config.GetType()));
            

        }


        static public IdContainer GetUniqueId(IdContainer id)
        {
            throw new NotImplementedException();
        }


        static public IdContainer GetUniqueId(EntityDefinition entityDefinition)
        {
            if (Instance.entity2uniqueIds.TryGetValue(entityDefinition.GetType(), out IdContainer uId))
            {
                return uId;
            }
            return entityDefinition.GetId();
        }


        static internal void AddUniqueId(EntityDefinition entityDefinition, UserInfo userInfo)
        {

            var Id = entityDefinition.GetId();

            var configType = entityDefinition.ConfigType();
            Instance.configIds.TryAdd(configType, new HashSet<IdContainer>());
            var ids = Instance.configIds[configType];



            if (ids.Contains(Id))
            {

                throw new NotImplementedException($"Uniquefying ids is not supported yet. {userInfo.GUID} is trying to register {entityDefinition.TemplateType().Name} id {Id} which already used by either vanilla entities or other mods.");

                if (!Instance.entity2uniqueIds.ContainsKey(entityDefinition.GetType()))
                {
                    var uId = MakeUniqueId(Id, entityDefinition, userInfo);
                    Instance.entity2uniqueIds.Add(entityDefinition.GetType(), uId);
                    ids.Add(uId);
                }
                else 
                {


                    throw new ArgumentException($"{entityDefinition.GetType()} already has a unique id"); 
                }
            }
            else
            {
                ids.Add(Id);
            }

        }

        

        static internal IdContainer MakeUniqueId(IdContainer id, EntityDefinition entityDefinition, UserInfo userInfo)
        {
            if (id.idType == IdContainer.IdType.String)
            {
                var uId = userInfo.GUID + id;
                // for fringe case of plugin have non-unique ids internally
                if (Instance.configIds[entityDefinition.ConfigType()].Contains(uId))
                    uId = uId + Instance.uIdSalt.Next().ToString();
                return uId;

            }

            if (id.idType == IdContainer.IdType.Int)
                throw new NotImplementedException();
            throw new NotImplementedException();
            
        }

        static internal int AddUniqueIndex(int index, EntityDefinition entityDefinition)
        {
            int i = Instance.indexTable.Sequence(entityDefinition.ConfigType()).Counter;
            var indexes = Instance.configIndexes[entityDefinition.ConfigType()];
            // 2do optimize this shit
            while (indexes.Contains(index + i))
            {
                //Log.LogDev()?.LogDebug($"MakeUniqueIndex: duplicate index {index + i} in {entityDefinition.ConfigType().Name} found and handled.");
                i = Instance.indexTable.Next(entityDefinition.ConfigType());
            }   

            indexes.Add(index + i);
            return index + i;

        }
            
        
    }
}
