using HarmonyLib;
using LBoL.Base;
using LBoL.Core;
using LBoL.Core.Cards;
using LBoL.Core.StatusEffects;
using LBoL.EntityLib.EnemyUnits.Character;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;

namespace LBoLEntitySideloader.CustomKeywords
{
    /// <summary>
    /// API for adding/removing/checking custom Keywords.
    /// </summary>
    public static class KeywordManager
    {
        static ConditionalWeakTable<Card, Dictionary<string, CardKeyword>> cwt_keywords = new ConditionalWeakTable<Card, Dictionary<string, CardKeyword>>();


        static Dictionary<string, CardKeyword> GetKeywords(Card card) {
            return cwt_keywords.GetValue(card, (_) => new Dictionary<string, CardKeyword>());
        }

        public static IEnumerable<CardKeyword> AllCustomKeywords(this Card card)
        {
            if(cwt_keywords.TryGetValue(card, out Dictionary<string, CardKeyword> keywords))
                return keywords.Values;
            return new HashSet<CardKeyword>();
        }

        public static bool HasCustomKeyword(this Card card, string kwId) 
        {
            return GetKeywords(card).ContainsKey(kwId);
        }

        public static void AddCustomKeyword(this Card card, CardKeyword keyword)
        {
            if (GetKeywords(card).TryGetValue(keyword.kwSEid, out var oldKw))
            {
                oldKw.Merge(keyword);
                return;
            }
            GetKeywords(card).Add(keyword.kwSEid, keyword);
        }

        public static void AddCustomKeyword<T>(this Card card, T keyword) where T : CardKeyword
        {
            AddCustomKeyword(card, (CardKeyword)keyword);
        }

        public static bool RemoveCustomKeyword(this Card card, CardKeyword keyword)
        {
            return GetKeywords(card).Remove(keyword.kwSEid);
        }


        public static bool TryGetCustomKeyword(this Card card, string kwId, out CardKeyword rezKeyword)
        {
            return GetKeywords(card).TryGetValue(kwId, out rezKeyword);
        }

        public static bool TryGetCustomKeyword<T>(this Card card, string kwId, out T rezKeyword) where T : CardKeyword
        {
            var rez = TryGetCustomKeyword(card, kwId, out var foundKw);
            rezKeyword = (T)foundKw;
            return rez;
        }

        public static CardKeyword GetCustomKeyword(this Card card, string kwId)
        {
            card.TryGetCustomKeyword(kwId, out var rezKeyword);
            return rezKeyword;
        }

        public static T GetCustomKeyword<T>(this Card card, string kwId) where T : CardKeyword
        {
            return GetCustomKeyword(card, kwId) as T;
        }


        [HarmonyPatch(typeof(Card), nameof(Card.EnumerateDisplayWords), MethodType.Enumerator)]
        class Card_Tooltip_Patch
        {

            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, MethodBase originalMethod)
            {
                return new CodeMatcher(instructions)
                    .MatchEndForward(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Library), nameof(Library.InternalEnumerateDisplayWords))))
                    .MatchEndBackwards(OpCodes.Ldloc_S)
                    .Advance(1)
                    .InsertAndAdvance(new CodeInstruction(OpCodes.Ldarg_0))
                    .InsertAndAdvance(new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(originalMethod.DeclaringType, "verbose")))
                    .InsertAndAdvance(new CodeInstruction(OpCodes.Ldloc_2))

                    .InsertAndAdvance(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Card_Tooltip_Patch), nameof(Card_Tooltip_Patch.AppendCustomKeywords))))

                    .MatchStartForward(OpCodes.Ldloc_S, OpCodes.Stfld)
                    .Advance(1)
                    .InsertAndAdvance(new CodeInstruction(OpCodes.Ldloc_2))
                    .InsertAndAdvance(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Card_Tooltip_Patch), nameof(Card_Tooltip_Patch.InvokeOnTooltipDisplay))))


                    .InstructionEnumeration();
            }

            private static IDisplayWord InvokeOnTooltipDisplay(IDisplayWord displayWord, Card card)
            {
                if (displayWord is IOnTooltipDisplay display)
                {
                    display.OnTooltipDisplay(card);
                }
                return displayWord;
            }

            private static IEnumerable<string> AppendCustomKeywords(IReadOnlyList<string> keywords, bool displayVerbose, Card card)
            {
                return keywords.Concat(KeywordManager.AllCustomKeywords(card).Where(kw => !kw.isVerbose || displayVerbose).Select(kw => kw.kwSEid));
            }
        }



        [HarmonyPatch(typeof(Card), nameof(Card.EnumerateAutoAppendKeywordNames))]
        class Card_Description_Patch
        {
            static void Postfix(Card __instance, ref IEnumerable<string> __result)
            {

                var card = __instance;
                var kwToAppend = card.AllCustomKeywords().Where(kw => kw.descPos != KwDescPos.DoNotDisplay)
                    .GroupBy(kw => kw.descPos, kw =>
                    {
                        if (kw.hasExtendedKeywordName)
                        {
                            var kwSe = TypeFactory<StatusEffect>.CreateInstance(kw.kwSEid);
                            kwSe.SourceCard = card;

                            if (kwSe is IExtendedKeywordName extendedKeywordName)
                            {
                                return extendedKeywordName.ExtendedKeywordName(card);
                            }
                            else
                            {
                                Log.LogDev()?.LogWarning($"Keyword {nameof(kw)} expects extended keyword name but {kw.kwSEid} does not implement {nameof(IExtendedKeywordName)}");
                            }
                        }

                        return TypeFactory<StatusEffect>.LocalizeProperty(kw.kwSEid, "Name", true, false);
                    });

                __result = kwToAppend.SelectMany(g => g.Key == KwDescPos.First ? g : Enumerable.Empty<string>())
                    .Concat(__result)
                    .Concat(kwToAppend.SelectMany(g => g.Key == KwDescPos.Last ? g : Enumerable.Empty<string>()));

            }
        }



        [HarmonyPatch(typeof(Card), nameof(Card.Description), MethodType.Getter)]
        class DescCheckCustomKw_Patch
        {

            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
            {
                return new CodeMatcher(instructions)
                    .MatchEndForward(new CodeInstruction(OpCodes.Call, AccessTools.DeclaredPropertyGetter(typeof(Card), nameof(Card.Keywords))))
                    .Advance(1)
                    .InsertAndAdvance(new CodeInstruction(OpCodes.Ldarg_0))
                    .InsertAndAdvance(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(DescCheckCustomKw_Patch), nameof(DescCheckCustomKw_Patch.CheckCustomKws))))
                    
                    .InstructionEnumeration();
            }

            private static bool CheckCustomKws(Keyword keywords, Card card)
            {
                return keywords != Keyword.None || card.AllCustomKeywords().Count() > 0;
            }
        }


        [HarmonyPatch(typeof(StatusEffect), nameof(StatusEffect.Brief), MethodType.Getter)]
        class StatusEffect_Brief_Patch
        {
            static void Postfix(StatusEffect __instance, ref string __result)
            {
                if (__instance is IOverrideSEBrief briefOverride)
                    __result = briefOverride.OverrideBrief(__result);
            }
        }



    }
}
