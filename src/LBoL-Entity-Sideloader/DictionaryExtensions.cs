using Cysharp.Threading.Tasks;
using LBoL.Base;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace LBoLEntitySideloader
{
    public static class DictionaryExtensions
    {
        // was overwritten?
        public static bool AlwaysAdd<K, V>(this Dictionary<K, V> dictionary, K key, V value)
        {
            if (!dictionary.TryAdd(key, value))
            {
                dictionary[key] = value;
                return true;
            }
            return false;
        }

        public static bool AlwaysAdd<K, V>(this AssociationList<K, V> associationList, K key, V value)
        {
            if (!associationList.TryAdd(key, value))
            {
                associationList[key] = value;
                return true;
            }
            return false;
        }

        // were duplicates found?
        public static bool Merge<K, V>(this Dictionary<K, V> dictionary, Dictionary<K, V> otherDic, bool overwrite = true)
        {
            bool dupes = false;
            otherDic.ToList().ForEach(x => dupes = dictionary.AlwaysAdd(x.Key, x.Value) ? true : dupes);
            return dupes;
        }


/*        public static void InnerAdd<K, V>(this Dictionary<K, V> dictionary, K outerKey, object innerKey, object value) where V : ICollection<KeyValuePair<object, object>>, new()
        {
            dictionary.TryAdd(outerKey, new V());
            dictionary[outerKey].Add(innerKey, value);
        }*/
    }
}
