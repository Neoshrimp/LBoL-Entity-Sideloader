using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace LBoLEntitySideloader
{
    internal class UniqueIdTracker
    {


        // config type => used Ids
        static public Dictionary<Type, HashSet<string>> configIds = new Dictionary<Type, HashSet<string>>();

        static public Dictionary<Type, HashSet<int?>> configIndexes = new Dictionary<Type, HashSet<int?>>();


        static void AddConfig(object config)
        {
            
        }    

    }
}
