using Cysharp.Threading.Tasks;
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

        // were duplicates found?
        public static bool Merge<K, V>(this Dictionary<K, V> dictionary, Dictionary<K, V> otherDic, bool overwrite = true)
        {
            bool dupes = false;
            otherDic.ToList().ForEach(x => dupes = dictionary.AlwaysAdd(x.Key, x.Value) ? true : dupes);
            return dupes;
        }
    }
}
