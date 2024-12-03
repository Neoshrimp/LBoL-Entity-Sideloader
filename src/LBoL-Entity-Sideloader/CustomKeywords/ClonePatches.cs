using HarmonyLib;
using LBoL.Core.Cards;

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace LBoLEntitySideloader.CustomKeywords
{
    [HarmonyPatch]
    class Clone_Patch
    {

        [HarmonyPatch(typeof(Card), nameof(Card.Clone), new Type[] { typeof(bool) })]
        [HarmonyPostfix]
        static void NonBattle(Card __instance, Card __result)
        {
            CopyKeywords(__instance, __result, CloningMethod.NonBattle);
        }

        [HarmonyPatch(typeof(Card), nameof(Card.CloneBattleCard))]
        [HarmonyPostfix]
        static void Copy(Card __instance, Card __result)
        {
            CopyKeywords(__instance, __result, CloningMethod.Copy);
        }

        [HarmonyPatch(typeof(Card), nameof(Card.CloneTwiceToken))]
        [HarmonyPostfix]
        static void TwiceToken(Card __instance, Card __result)
        {
            CopyKeywords(__instance, __result, CloningMethod.TwiceToken);
        }

        static void CopyKeywords(Card og, Card copy, CloningMethod cloningMethod)
        {

            foreach (var kw in og.AllCustomKeywords())
            {
                var clone = kw.Clone(cloningMethod);
                if (clone != null)
                { 
                    copy.AddCustomKeyword(clone);
                }
            }
        }

    }

}
