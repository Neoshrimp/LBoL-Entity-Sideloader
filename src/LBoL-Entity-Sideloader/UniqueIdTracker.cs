using LBoLEntitySideloader.ReflectionHelpers;
using System;
using System.Collections.Generic;
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


        static public Dictionary<Type, HashSet<IdContainer>> modifiedIds = new Dictionary<Type, HashSet<IdContainer>>();


        //duplicate
        static public void AddConfig(object config, bool allowDuplicateIndex = false )
        {
            configIds.TryAdd(config.GetType(), new HashSet<IdContainer>());
            configIds.TryGetValue(config.GetType(), out HashSet<IdContainer> ids);

            var f_id = ConfigReflection.GetIdField(config.GetType());
            var id = (IdContainer)f_id.GetValue(config);

            if (ids.Contains(id))
            {
                log.LogDebug(id);
                throw new NotImplementedException();
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
                    throw new NotImplementedException();
                }
                else
                {
                    indexes.Add(index);
                }
            }
        }



    }
}
