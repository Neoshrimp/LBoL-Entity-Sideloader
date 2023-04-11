using LBoL.Presentation.UI.Widgets;
using LBoLEntitySideloader.ReflectionHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace LBoLEntitySideloader
{
    public class UniqueIdTracker
    {
        private static readonly BepInEx.Logging.ManualLogSource log = BepinexPlugin.log;

        // config type => used Ids
        static public Dictionary<Type, HashSet<IdContainer>> configIds = new Dictionary<Type, HashSet<IdContainer>>();

        static public Dictionary<Type, HashSet<int>> configIndexes = new Dictionary<Type, HashSet<int>>();

        // EntityDefinition type 
        static public Dictionary<Type, IdContainer> entity2uniqueIds = new Dictionary<Type, IdContainer>();

        
        
        static Sequence uIdSalt = new Sequence();

        static Sequence uIntId = new Sequence(1394);


        static TemplateSequenceTable indexTable = new TemplateSequenceTable(12000);

        static public Dictionary<Type, int> entity2uniqueIndexes = new Dictionary<Type, int>();

        //static private HashSet<IdContainer> uniqueIds = new HashSet<IdContainer>();



        static internal void TrackVanillaConfig(object config, bool allowDuplicateIndex = false )
        {
            configIds.TryAdd(config.GetType(), new HashSet<IdContainer>());
            configIds.TryGetValue(config.GetType(), out HashSet<IdContainer> ids);

            var f_id = ConfigReflection.GetIdField(config.GetType());


            IdContainer id = IdContainer.CastFromObject(f_id.GetValue(config));


            if (ids.Contains(id))
            {
                log.LogDebug(id);
                log.LogWarning($"duplicate id: {id} in {config.GetType()}");
            }
            else
            {
                ids.Add($"id: {id}");
            }

            var f_index = ConfigReflection.HasIndex(config.GetType());
            if (f_index != null)
            {
                configIndexes.TryAdd(config.GetType(), new HashSet<int>());
                configIndexes.TryGetValue(config.GetType(), out HashSet<int> indexes);

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
        }


        static public IdContainer GetUniqueId(IdContainer id)
        {
            throw new NotImplementedException();
        }


        static public IdContainer GetUniqueId(EntityDefinition entityDefinition)
        {
            if (entity2uniqueIds.TryGetValue(entityDefinition.GetType(), out IdContainer uId))
            {
                return uId;
            }
            return entityDefinition.GetId();
        }


        static internal void AddUniqueId(EntityDefinition entityDefinition, UserInfo userInfo)
        {

            var Id = entityDefinition.GetId();

            var configType = entityDefinition.ConfigType();
            configIds.TryAdd(configType, new HashSet<IdContainer>());
            var ids = configIds[configType];


            if (ids.Contains(Id))
            {

                throw new NotImplementedException($"Uniquefying ids is not supported yet. {userInfo.GUID} is trying to register {Id} which already used");

                if (!entity2uniqueIds.ContainsKey(entityDefinition.GetType()))
                {
                    var uId = MakeUniqueId(Id, entityDefinition, userInfo);
                    entity2uniqueIds.Add(entityDefinition.GetType(), uId);
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
                if (configIds[entityDefinition.ConfigType()].Contains(uId))
                    uId = uId + uIdSalt.Next().ToString();
                return uId;

            }



            if (id.idType == IdContainer.IdType.Int)
                throw new NotImplementedException();
            throw new NotImplementedException();
            
        }

        static internal int AddUniqueIndex(int index, EntityDefinition entityDefinition)
        {
            int i = indexTable.Next(entityDefinition.ConfigType());
            var indexes = configIndexes[entityDefinition.ConfigType()];
            while (indexes.Contains(index + i))
            {
                log.LogDebug($"MakeUniqueIndex: duplicate index{index + i} of {entityDefinition.ConfigType()}");
                i = indexTable.Next(entityDefinition.ConfigType());
            }

            indexes.Add(index + i);
            return index + i;

        }


    }
}
