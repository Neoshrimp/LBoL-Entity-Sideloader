using HarmonyLib;
using LBoL.Base.Extensions;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Adventures;
using LBoL.Core.Cards;
using LBoL.Core.GapOptions;
using LBoL.Core.StatusEffects;
using LBoL.Core.Units;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace LBoLEntitySideloader.ReflectionHelpers
{

    public class TypeFactoryReflection
    {
        // from Library.RegisterAllAsync. No sensible common pattern exists to extract these types reflectively
        public static List<Type> factoryTypes = new List<Type>() { typeof(Stage), typeof(Card), typeof(UltimateSkill), typeof(Exhibit), typeof(PlayerUnit), typeof(EnemyUnit), typeof(StatusEffect), typeof(Doll), typeof(GapOption), typeof(Intention), typeof(Adventure), typeof(JadeBox) };


/*        static HashSet<Type> _isSubclass = new HashSet<Type>(new AssignabilityComparer());

        public static HashSet<Type> IsSubclass 
        { 
            get 
            {
                if (_isSubclass.Empty())
                {
                    factoryTypes.Do(t => _isSubclass.Add(t));

                    _isSubclass.Do(t => UnityEngine.Debug.Log(t.Name));
                }

                return _isSubclass;
            }
                
        }*/
    }




    class AssignabilityComparer : IEqualityComparer<Type>
    {
        public bool Equals(Type x, Type y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }

            if (x == null || y == null)
            {
                return false;
            }

            return x.IsSubclassOf(y) || y.IsSubclassOf(x);
        }

        public int GetHashCode(Type obj)
        {
            return obj == null ? 0 : obj.GetHashCode();
        }
    }


}
