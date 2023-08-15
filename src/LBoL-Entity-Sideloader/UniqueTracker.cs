using LBoL.ConfigData;
using LBoL.Presentation.UI.Widgets;
using LBoLEntitySideloader.Entities;
using LBoLEntitySideloader.ReflectionHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

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
                    _instance = new UniqueTracker();
                return _instance;
            }
        }

        internal static void DestroySelf() { _instance = null; }


        // config type => used Ids
        public Dictionary<Type, HashSet<IdContainer>> configIds = new Dictionary<Type, HashSet<IdContainer>>();

        public Dictionary<Type, HashSet<int>> configIndexes = new Dictionary<Type, HashSet<int>>();

        // EntityDefinition type 
        public Dictionary<Type, IdContainer> entity2uniqueIds = new Dictionary<Type, IdContainer>();

        public Dictionary<Type, Dictionary<IdContainer, int>> id2ConfigListIndex = new Dictionary<Type, Dictionary<IdContainer, int>>();

        private TemplateSequenceTable tempConfigIndexTable = new TemplateSequenceTable();

        // definition ids
        public HashSet<Type> invalidRegistrations = new HashSet<Type>();

        // factype =>+ id =>+ component string =>+ OverwiteInfo(component, defType, userInfo) 
        public Dictionary<Type, Dictionary<IdContainer, Dictionary<string, OverwriteInfo>>> overwriteTracker = new Dictionary<Type, Dictionary<IdContainer, Dictionary<string, OverwriteInfo>>>();

        Sequence uIdSalt = new Sequence();

        Sequence uIntId = new Sequence(1394);


        TemplateSequenceTable indexTable = new TemplateSequenceTable(12000);

        public Dictionary<Type, int> entity2uniqueIndexes = new Dictionary<Type, int>();

        // templateType => Id => definitionType
        public Dictionary<Type, Dictionary<string, Type>> onDemandResourceTracker = new Dictionary<Type, Dictionary<string, Type>>();

        //static private HashSet<IdContainer> uniqueIds = new HashSet<IdContainer>();


        public static bool IsOverwriten(Type facType, IdContainer id, string component, Type definitionType, UserInfo user)
        {
            Instance.overwriteTracker.TryAdd(facType, new Dictionary<IdContainer, Dictionary<string, OverwriteInfo>>());
            var idDic = Instance.overwriteTracker[facType];
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

                throw new NotImplementedException($"Uniquefying ids is not supported yet. {userInfo.GUID} is trying to register {entityDefinition.EntityType().Name} type with id '{Id}' which already used by either vanilla entities or other mods.");

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
            int i = Instance.indexTable.Next(entityDefinition.ConfigType());
            var indexes = Instance.configIndexes[entityDefinition.ConfigType()];
            while (indexes.Contains(index + i))
            {
                Log.LogDevExtra()?.LogDebug($"(Extra Logging) MakeUniqueIndex: duplicate index {index + i} in {entityDefinition.ConfigType().Name} found and handled.");
                i = Instance.indexTable.Next(entityDefinition.ConfigType());
            }

            indexes.Add(index + i);
            return index + i;

        }

        
    }
}
