using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace LBoLEntitySideloader
{
    public static class DictionaryExtensions
    {
        // was added first try?
        public static bool AlwaysAdd<K, V>(this Dictionary<K, V> dictionary, K key, V value)
        {
            if (!dictionary.TryAdd(key, value))
            {
                dictionary[key] = value;
                return true;
            }
            return false;
        }
    }
}
