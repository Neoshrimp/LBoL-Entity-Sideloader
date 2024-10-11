using HarmonyLib;
using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Cards;
using LBoL.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Reflection;
using LBoL.Presentation.UI.Panels;
using LBoLEntitySideloader.ReflectionHelpers;
using System.Collections;
using LBoL.Core.Battle;
using LBoL.EntityLib.Exhibits.Shining;
using LBoL.EntityLib.EnemyUnits.Character;
using LBoL.Presentation.UI.ExtraWidgets;
using LBoL.Presentation;
using LBoL.EntityLib.Adventures;
using LBoL.Base.Extensions;
using LBoL.Core.StatusEffects;
using JetBrains.Annotations;
using Mono.CSharp;

namespace LBoLEntitySideloader.GameFixes
{

    /// <summary>
    /// Makes vfx and sfx optionally delayed in perform array
    /// </summary>
    [HarmonyPatch(typeof(Card), nameof(Card.CardPerformAction))]
    class CardPerformAction_Patch
    {

        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            int i = 0;
            var ciList = instructions.ToList();
            var c = ciList.Count();
            CodeInstruction prevCi = null;
            foreach (var ci in instructions)
            {
                if (ci.Is(OpCodes.Ldc_R4, 0f) && (ciList.ElementAtOrDefault(i + 5)?.Is(OpCodes.Call, AccessTools.Method(typeof(PerformAction), nameof(PerformAction.Effect))) ?? false))
                {
                    yield return new CodeInstruction(OpCodes.Ldloc_2);
                }

                else if (ci.Is(OpCodes.Ldc_R4, 0f) && (ciList.ElementAtOrDefault(i + 1)?.Is(OpCodes.Call, AccessTools.Method(typeof(PerformAction), nameof(PerformAction.Sfx))) ?? false))
                {

                    yield return new CodeInstruction(OpCodes.Ldloc_2);

                }
                else
                {
                    yield return ci;
                }
                prevCi = ci;
                i++;
            }
        }

    }







    [HarmonyPatch(typeof(GameRunController), nameof(GameRunController.BaseCardWeight))]
    class UncapColorLimitation_Patch
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            int i = 0;
            var ciList = instructions.ToList();
            var c = ciList.Count();
            CodeInstruction prevCi = null;
            int patchCount = 0;
            bool skipMode = false;
            CodeInstruction jmpSwitchEnd = null;
            CodeInstruction numStore = null;

            int patchCasesTotal = 2;
            foreach (var ci in instructions)
            {
                if (ci.Is(OpCodes.Ldstr, "Invalid cost pattern {0} of card '{1}'") && patchCount < patchCasesTotal)
                {
                    skipMode = true;
                    jmpSwitchEnd = prevCi;
                    numStore = ciList[i - 2];
                    // prevents deletion of instruction label
                    yield return ci;
                    yield return new CodeInstruction(OpCodes.Pop);
                }
                else if (skipMode && ci.opcode == OpCodes.Throw && patchCount < patchCasesTotal)
                {
                    // trivialColorCount case 1
                    if (patchCount == 0)
                    {
                        yield return new CodeInstruction(OpCodes.Ldc_R4, 1.2f);
                    }
                    // trivialColorCount case 2
                    else if (patchCount == 1)
                    {
                        yield return new CodeInstruction(OpCodes.Ldc_R4, 1.2f);

                    }
                    yield return numStore;
                    yield return jmpSwitchEnd;

                    skipMode = false;
                    patchCount += 1;
                }
                else if (!skipMode)
                {
                    yield return ci;
                }
                prevCi = ci;
                i++;
            }
        }

    }

    [HarmonyPatch(typeof(GameRunController), nameof(GameRunController.BaseCardWeight))]
    [HarmonyPriority(Priority.LowerThanNormal)]
    class UncapColorLimitation_Patch2
    {
        static float SetWeight(int trivialColors)
        {
            var num = trivialColors switch
            {
                4 => 1.3f,
                5 => 1.4f,
                _ => 1f,
            };
            return num;
        }

        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            return new CodeMatcher(instructions)
                .MatchForward(true, new CodeMatch[] { OpCodes.Call, OpCodes.Call, OpCodes.Stloc_S })
                .InsertAndAdvance(new CodeInstruction(OpCodes.Dup))
                .InsertAndAdvance(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(UncapColorLimitation_Patch2), nameof(UncapColorLimitation_Patch2.SetWeight))))
                .InsertAndAdvance(new CodeInstruction(OpCodes.Stloc_3))

                .MatchForward(true, OpCodes.Throw)
                .Set(OpCodes.Pop, null)

                .InstructionEnumeration();
        }
    }

    [HarmonyPatch(typeof(CardUi), nameof(CardUi.Awake))]
    class _13thCard_Patch
    {

        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            CodeInstruction prevCi = null;
            foreach (var ci in instructions)
            {
                if (ci.opcode == OpCodes.Ldc_I4_S && prevCi.opcode == OpCodes.Ldloc_0)
                {
                    yield return new CodeInstruction(OpCodes.Ldc_I4, 99);
                }
                else
                {
                    yield return ci;
                }
                prevCi = ci;
            }

        }
    }
    


    // makes 3 fairies a bit more compatible with being grouped with other enemies
    [HarmonyPatch(typeof(Sunny), "OnEnterBattle")]
    class SunnyCastBug_Patch
    {

        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {

            foreach (var ci in instructions)
            {
                if (ci.Is(OpCodes.Castclass, typeof(LightFairy)))
                {
                    yield return new CodeInstruction(OpCodes.Isinst, typeof(LightFairy));
                }
                else
                {
                    yield return ci;
                }
            }
        }

    }


    // might cause issues with future updates. Only ReimuR is actually using the spell cache
    [HarmonyPatch(typeof(SpellPanel), nameof(SpellPanel.Awake))]
    class SpellPanel_ClearSpellCache_Patch
    {

        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            return new CodeMatcher(instructions, generator)
                .End()
                .MatchBack(true, new CodeMatch[] { OpCodes.Ldloc_1, OpCodes.Ldloc_0, OpCodes.Ldlen, OpCodes.Conv_I4, OpCodes.Blt })
                .Advance(1)
                .CreateLabel(out var afterLoopLabel)
                .AddLabels(new Label[] { afterLoopLabel })
                .Start()
                .MatchForward(false, new CodeMatch(OpCodes.Ldstr, "UI/Panels/SpellDeclare"))
                .Insert(new CodeInstruction(OpCodes.Br, afterLoopLabel))
                .InstructionEnumeration();
        }

    }



    // 2do mysterious bug. cannot reproduce
    [HarmonyPatch(typeof(CardUi), nameof(CardUi.SetPendingCardsAlpha))]
    class CardUi_SetPendingCardsAlpha_Patch
    {
        static bool Prefix(CardUi __instance, float alpha)
        {
            try
            {
                if(__instance._pendingUseWidgets != null)
                    foreach (HandCard handCard in __instance._pendingUseWidgets)
                    {
                        if(handCard?.CanvasGroup?.alpha != null)
                            handCard.CanvasGroup.alpha = alpha;
                    }
            }
            catch (Exception ex)
            {

                Log.log.LogWarning(ex);
            }


            return false;
        }

    }


    // 2do keep error handling
    [HarmonyPatch(typeof(AudioManager), nameof(AudioManager.EnterStage))]
    class AudioManagerEnterStage_Patch
    {

        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            return new CodeMatcher(instructions, generator)
                .MatchForward(false, new CodeMatch(OpCodes.Ret))
                .RemoveInstruction()
                .InstructionEnumeration();
        }

    }

    // Make CantLose exhibits actually can't lose
    [HarmonyPatch(typeof(Debut), nameof(Debut.ExchangeExhibit))]
    class DebutCantLoseFix_Patch
    {
        static bool Prefix(Debut __instance)
        {
            if (GameMaster.Instance.CurrentGameRun?.Player.Exhibits.FirstOrDefault()?.LosableType == ExhibitLosableType.CantLose)
            {
                return false;
            }
            return true;
        }
    }

    // Fix Seija's damage limiter to work properly
    [HarmonyPatch(typeof(LimitedDamage), nameof(LimitedDamage.OnDamageReceived))]
    class LimitedDamageFix_Patch
    {
        static bool Prefix(LimitedDamage __instance, ref DamageEventArgs args)
        {
            __instance.Count -= Math.Min(args.DamageInfo.Damage.RoundToInt(), __instance.Count);
            return false;
        }
    }
}
