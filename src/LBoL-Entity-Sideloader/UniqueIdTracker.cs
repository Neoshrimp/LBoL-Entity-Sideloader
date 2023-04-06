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
            if (entity2uniqueIds.TryGetValue(entityDefinition.GetConfigType(), out IdContainer uId))
            {
                return uId;
            }
            if (entityDefinition.id != default)
            {
                return entityDefinition.id;
            }
            throw new ArgumentException($"GetUniqueId: {entityDefinition} doesn't have an id");
        }


        static internal void AddUniqueId(IdContainer id, EntityDefinition entityDefinition, UserInfo userInfo)
        {

            log.LogDebug($"AddUniqueId: {entityDefinition.GetType().Name} {id}");
            var configType = entityDefinition.GetConfigType();
            configIds.TryAdd(configType, new HashSet<IdContainer>());
            var ids = configIds[configType];




            //if (ids.Contains(id))
            if (ids.FirstOrDefault(id => id.SId == id) != default(IdContainer))
            {

                log.LogDebug($"deeznuts cvontains");

                if (!entity2uniqueIds.ContainsKey(entityDefinition.GetType()))
                {
                    log.LogDebug($"deeznuts");
                    var uId = MakeUniqueId(id, userInfo);
                    entity2uniqueIds.Add(entityDefinition.GetType(), uId);
                    ids.Add(uId);
                }
                else 
                {
                    log.LogWarning($"{entityDefinition.GetType()} already has a unique id");
                }
            }
            else
            {
                ids.Add(id);
            }

        }


        static internal IdContainer MakeUniqueId(IdContainer id, UserInfo userInfo)
        {
            if (id.idType == IdContainer.IdType.String)
                return userInfo.GUID + id;
            throw new NotImplementedException();
        }

        static internal int MakeUniqueIndex(int index, UserInfo userInfo)
        {
            throw new NotImplementedException();
        }


    }
}
