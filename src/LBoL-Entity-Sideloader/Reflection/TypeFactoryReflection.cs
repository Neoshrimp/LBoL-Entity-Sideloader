using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Adventures;
using LBoL.Core.GapOptions;
using LBoL.Core.StatusEffects;
using LBoL.Core.Units;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace LBoLEntitySideloader.ReflectionHelpers
{

    public class TypeFactoryReflection
    {
        // from Library.RegisterAllAsync. No sensible common pattern exists to extract these types reflectively
        public static HashSet<Type> factoryTypes = new HashSet<Type>() { typeof(Stage), typeof(CardConfig), typeof(UltimateSkill), typeof(Exhibit), typeof(PlayerUnit), typeof(EnemyUnit), typeof(StatusEffect), typeof(Doll), typeof(GapOption), typeof(Intention), typeof(Adventure), typeof(JadeBox) };
    }
    

}
