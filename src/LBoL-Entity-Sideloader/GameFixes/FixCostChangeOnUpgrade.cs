using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using LBoL.Base;
using LBoL.Core.Cards;

namespace LBoLEntitySideloader.GameFixes
{

    // patch to fix interaction between base/temporary cost reduction and in-battle upgrades
    [HarmonyPatch]
    static class Cost
    {
        [HarmonyPatch(typeof(Card), nameof(Card.CostChangeInUpgrading)), HarmonyPrefix]
        private static bool FixCostChange(Card __instance)
        {
            if (__instance.IsXCost || null == __instance.Config.UpgradedCost)
            {
                return false;
            }

            ManaGroup configDecrease =
                __instance.Config.Cost - __instance.Config.UpgradedCost.Value;

            if (ManaGroup.Empty == configDecrease)
            {
                return false;
            }

            // final correction
            ManaGroup baseCorrection = __instance.Config.Cost != __instance.BaseCost
                    ? (__instance.BaseCost - configDecrease).GetCostCorrection()
                    : ManaGroup.Empty;
            ManaGroup turnCorrection = ManaGroup.Empty != __instance.TurnCostDelta
                    ? (__instance.BaseCost - configDecrease - baseCorrection + __instance.TurnCostDelta).GetCostCorrection()
                    : ManaGroup.Empty;

            __instance.DecreaseBaseCost(configDecrease + baseCorrection);
            __instance.DecreaseTurnCost(turnCorrection);
            return false;
        }
    }




    public static class ManaGroupExtensions
    {
        public static ManaGroup GetCostCorrection(this ManaGroup delta)
        {
            ManaGroup surplus = (delta * (-1)).Corrected;
            ManaGroup deficit = delta.Corrected;

            ManaGroup result = ManaGroup.Empty;

            // redistribute surplus to hybrid/any mana in deficit
            if (0 < deficit.Hybrid)
            {
                foreach (ManaColor c in deficit.GetHybridColors)
                {
                    int pips = Math.Min(surplus[c], deficit.Hybrid);

                    surplus -= ManaGroup.FromColor(c, pips);
                    result -= ManaGroup.FromColor(c, pips);

                    deficit -= ManaGroup.FromColor(ManaColor.Hybrid, pips);
                    result += ManaGroup.FromColor(ManaColor.Hybrid, pips);
                }
            }

            foreach (ManaColor c in surplus.EnumerateColors())
            {
                int pips = Math.Min(deficit[ManaColor.Any], surplus[c]);

                surplus -= ManaGroup.FromColor(c, pips);
                result -= ManaGroup.FromColor(c, pips);

                deficit -= ManaGroup.Anys(pips);
                result += ManaGroup.Anys(pips);
            }

            return result;
        }
    }




}
