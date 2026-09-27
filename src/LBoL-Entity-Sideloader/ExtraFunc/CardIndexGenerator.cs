using BepInEx;
using LBoL.Base;
using LBoL.ConfigData;
using LBoL.EntityLib.EnemyUnits.Lore;
using LBoLEntitySideloader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LBoLEntitySideloader.ExtraFunc
{
    /// <summary>
    /// Class for automatically generating indices for cards.
    /// <para>
    /// If you have a character mod, make sure you have a public static List&lt;<see cref="ManaColor"/>&gt; offColors in your bepinex.  
    /// </para>
    /// Currently doesn't support two character mods.
    /// </summary>
    public static class CardIndexGenerator
    {
        private static readonly Dictionary<Assembly, int> offsetPerAssembly = new Dictionary<Assembly, int>();
        internal static IReadOnlyList<ManaColor> CurrentOffColors = Array.Empty<ManaColor>();
        internal static Assembly CurrentAssembly;

        public const int milx1 = (int)1E7;

        private static HashSet<int> uniqueIds = new HashSet<int>();
        public static HashSet<int> UniqueIds { get { if (uniqueIds == null) uniqueIds = new HashSet<int>(); return uniqueIds; } }
        internal static void PromiseClearIndexSet() => EntityManager.AddPostLoadAction(() => uniqueIds = null);

        private static int GetOrComputeOffset(Assembly assembly)
        {
            if (!offsetPerAssembly.TryGetValue(assembly, out var offset))
            {
                int millions = 0;
                if (UniqueTracker.Instance.configIndexes.TryGetValue(typeof(CardConfig), out var indexSet))
                    millions = indexSet.Where(i => i >= milx1).DefaultIfEmpty().Max() / milx1;
                offset = (millions + 1) * milx1;
                offsetPerAssembly[assembly] = offset;
            }
            return offset;
        }

        /// <summary>
        /// Generate a unique card ID based on the original offset, the card's rarity, colors and the type:
        /// <para>
        /// Example:
        ///    initial offset = 50000000<br/>
        ///    rarity = Common<br/>
        ///    color = Blue(Main Color)<br/>
        ///    cost = 3<br/>
        ///    type = Attack<br/>
        ///    ID = 50123101<br/>
        ///    0: Blue is a main color(0=Main Color, 1=Off-Color)<br/>
        ///    1: Common(Basic= 0, Common= 1, Uncommon= 2, Rare= 2)<br/>
        ///    2: Blue(White= 1, Blue= 2, Black= 3, Red= 4, Green= 5, Colorless= 6, Multicolor= 9)<br/>
        ///    3: Card's cost (Unplayable, X-cost and Cost > 9 will return 9)<br/>
        ///    1: Attack(Attack = 1, Defense = 2, Skill = 3, Ability = 4, Teammate = 5)<br/>
        ///    01: Amount of cards with the same value whose IDs were set before.<br/>
        /// </para>
        /// </summary>
        /// <param name="config">The card configuration to index.</param>
        /// <returns>A unique index for the card.</returns>
        public static int GetUniqueIndex(CardConfig config)
        {

            var offColors = CurrentOffColors ?? Array.Empty<ManaColor>();
            int id = GetOrComputeOffset(CurrentAssembly);

            //Off-color check
            id += config.Colors.Any(offColors.Contains) ? 1000000 : 0;

            //Rarity
            id += config.Keywords.HasFlag(Keyword.Basic) ? 0 : (int)(config.Rarity + 1) * 100000;

            //Color
            int color;
            if (config.Colors.Count > 1)
                color = 9;
            else if (config.Colors.Count == 0)
                color = (int)ManaColor.Colorless;
            else
                color = (int)config.Colors[0];

            id += color * 10000;

            //Cost
            int cost = config.IsXCost || config.Keywords.HasFlag(Keyword.Forbidden) || config.Cost.Total > 9 ? 9 : config.Cost.Total;
            id += cost * 1000;

            //Type
            id += (int)config.Type * 100;

            //Cards with similar parameters
            if (UniqueTracker.Instance.configIndexes.TryGetValue(typeof(CardConfig), out var idxSet))
                while (idxSet.Contains(id)) id++;

            return id;
        }
    }
}