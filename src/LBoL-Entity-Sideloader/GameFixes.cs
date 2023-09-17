using HarmonyLib;
using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Cards;
using LBoL.Core.Units;
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
using UnityEngine;
using LBoL.Core.Battle;
using LBoL.EntityLib.Exhibits.Shining;
using LBoL.EntityLib.EnemyUnits.Character;

namespace LBoLEntitySideloader
{
    public class GameFixes
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

            // cba'd to transpile jump table
            static bool Prefix(ref float __result, CardConfig config, bool applyFactors, GameRunController __instance)
            {

                int trivialColorCount = Math.Max(config.Colors.Count, config.Cost.TrivialColorCount);
                if (trivialColorCount <= 3)
                    return true;

                float num;

                switch (trivialColorCount)
                {
                    case 4:
                        num = 1.3f;
                        break;
                    case 5:
                        num = 1.4f;
                        break;
                    default:
                        throw new InvalidDataException($"{trivialColorCount} is too many colors in either {config.Cost} or {config.Colors} of card {config.Id}");
                }


                float num3 = num;
                float num4 = 1f;
                int count = config.Colors.Count;
                if (count <= 0)
                {
                    if (count == 0)
                    {
                        num4 = 0.8f;
                    }
                }
                else
                {
                    foreach (ManaColor manaColor in config.Colors)
                    {
                        float num5 = (float)__instance.BaseMana.GetValue(manaColor) / (float)__instance.BaseMana.Amount;
                        num5 -= 0.5f;
                        num5 *= 0.8f;
                        num4 += num5;
                    }
                    num4 = Math.Max(num4, 0.8f);
                }
                num3 *= num4;
                if (applyFactors)
                {
                    if (config.Rarity == Rarity.Rare)
                    {
                        num3 *= __instance._cardRareWeightFactor;
                    }
                    float num6;
                    if (__instance._cardRewardWeightFactors.TryGetValue(config.Id, out num6))
                    {
                        num3 *= num6;
                    }
                }
                __result = num3;

                return false;
            }


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



        [HarmonyPatch]
        class ViewConsumeMana_ErrorMessage_Patch
        {


            static IEnumerable<MethodBase> TargetMethods()
            {
                yield return ExtraAccess.InnerMoveNext(typeof(BattleManaPanel), nameof(BattleManaPanel.ViewConsumeMana));
            }



            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {

                CodeInstruction prevCi = null;
                foreach (var ci in instructions)
                {
                    // 'fix' for harmonyX bug
                    if (ci.opcode == OpCodes.Leave)
                    {
                        yield return ci;
                        yield return new CodeInstruction(OpCodes.Nop);
                    }
                    else if (prevCi != null && ci.opcode == OpCodes.Call && prevCi.opcode == OpCodes.Ldstr && prevCi.operand.ToString() == "Cannot dequeue consuming mana, resetting all.")
                    {

                        yield return new CodeInstruction(OpCodes.Pop);
                    }
                    else
                    {
                        yield return ci;
                    }
                    prevCi = ci;
                }
            }

        }


        // makes 3 fairies a bit more compatible with being grouped with another enemies
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
                    .MatchBack(true, new CodeMatch[] { OpCodes.Ldloc_1, OpCodes.Ldloc_0, OpCodes.Ldlen, OpCodes.Conv_I4, OpCodes.Blt})
                    .Advance(1)
                    .CreateLabel(out var afterLoopLabel)
                    .AddLabels(new Label[] { afterLoopLabel })
                    .Start()
                    .MatchForward(false, new CodeMatch(OpCodes.Ldstr, "UI/Panels/SpellDeclare"))
                    .Insert(new CodeInstruction(OpCodes.Br, afterLoopLabel))
                    .InstructionEnumeration();
            }

        }



        // patches to fix card panel 
        class CardView_Patches
        {

            // static state variable should be good enough since there's only ever one mini cardSelect panel
            static public bool miniPanelShowing = false;


            [HarmonyPatch(typeof(SelectCardPanel), nameof(SelectCardPanel.ViewMiniSelect))]
            class ViewMiniSelect_Patch
            {

                static void Prefix()
                {
                    miniPanelShowing = true;
                }
            }



            [HarmonyPatch]
            class SelectCardPanel_Patch
            {


                static IEnumerable<MethodBase> TargetMethods()
                {
                    yield return AccessTools.Method(typeof(SelectCardPanel), nameof(SelectCardPanel.ViewSelectHand));
                    yield return AccessTools.Method(typeof(SelectCardPanel), nameof(SelectCardPanel.ViewSelectCard));

                }


                // miniSelect get deactivated on these methods
                static void Prefix()
                {
                    miniPanelShowing = false;
                }

            }




            [HarmonyPatch]
            class MiniPanelFadeDelegate_Patch
            {

                static IEnumerable<MethodBase> TargetMethods()
                {
                    yield return AccessTools.Method(typeof(SelectCardPanel).GetNestedTypes(AccessTools.allDeclared).Single(t => t.Name.Contains("DisplayClass63_0")), "<ViewMiniSelect>b__0");
                }



                static bool PreCon()
                {
                    return miniPanelShowing;
                }

                static void PostFlip()
                {
                    miniPanelShowing = false;
                }

                static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
                {


                    return new CodeMatcher(instructions)
                        .MatchForward(false, new CodeMatch(OpCodes.Callvirt, AccessTools.Method(typeof(GameObject), nameof(GameObject.SetActive))))
                        .Advance(-1)
                        .Set(OpCodes.Call, AccessTools.Method(typeof(MiniPanelFadeDelegate_Patch), nameof(MiniPanelFadeDelegate_Patch.PreCon)))
                        .Advance(2)
                        .Insert(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(MiniPanelFadeDelegate_Patch), nameof(MiniPanelFadeDelegate_Patch.PostFlip))))
                        .InstructionEnumeration();

                }


            }

        }


        [HarmonyPatch]
        class AllowDuplicateExhibits1_Patch
        {
            static IEnumerable<MethodBase> TargetMethods()
            {
                yield return ExtraAccess.InnerMoveNext(typeof(GameRunController), nameof(GameRunController.GainExhibitRunner));
            }
            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
            {
                CodeInstruction prevprevCi = null;
                CodeInstruction prevCi = null;

                var logErrorMethod = AccessTools.Method(typeof(Debug), nameof(Debug.LogError), new Type[] { typeof(object) });
                foreach (var ci in instructions)
                {
                    if (prevprevCi != null && prevCi != null && ci.opcode == OpCodes.Call && ((MethodInfo)ci.operand) == logErrorMethod && prevCi.opcode == OpCodes.Call && prevprevCi.opcode == OpCodes.Callvirt)
                    {
                        yield return new CodeInstruction(OpCodes.Pop);
                    }
                    else if (prevprevCi != null && prevCi != null && ci.opcode == OpCodes.Ret && prevCi.opcode == OpCodes.Ldc_I4_0 && prevprevCi.opcode == OpCodes.Call && ((MethodInfo)prevprevCi.operand) == logErrorMethod)
                    {
                        yield return new CodeInstruction(OpCodes.Pop);
                    }
                    else if (prevprevCi != null && prevCi != null && ci.opcode == OpCodes.Newobj && prevCi.opcode == OpCodes.Ldstr && prevprevCi.opcode == OpCodes.Ldstr && prevprevCi.operand.ToString() == "Cannot add duplicated Exhibit.")
                    {
                        yield return new CodeInstruction(OpCodes.Pop);
                    }
                    else if (prevprevCi != null && prevCi != null && ci.opcode == OpCodes.Throw && prevCi.opcode == OpCodes.Newobj && prevprevCi.opcode == OpCodes.Ldstr && prevprevCi.operand.ToString() == "exhibit")
                    {
                        yield return new CodeInstruction(OpCodes.Pop);
                    }
                    else
                    {
                        yield return ci;
                    }
                    prevprevCi = prevCi;
                    prevCi = ci;
                }
            }
        }

        [HarmonyPatch(typeof(PlayerUnit), nameof(PlayerUnit.AddExhibit))]
        class AllowDuplicateExhibits2_Patch
        {
            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                CodeInstruction prevprevCi = null;
                CodeInstruction prevCi = null;
                foreach (var ci in instructions)
                {
                    if (prevprevCi != null && prevCi != null && ci.opcode == OpCodes.Throw && prevCi.opcode == OpCodes.Newobj && prevprevCi.opcode == OpCodes.Ldstr && prevprevCi.operand.ToString() == "exhibit")
                    {
                        yield return new CodeInstruction(OpCodes.Pop);
                    }
                    else if (prevprevCi != null && prevCi != null && ci.opcode == OpCodes.Newobj && prevCi.opcode == OpCodes.Ldstr && prevprevCi.opcode == OpCodes.Ldstr && prevprevCi.operand.ToString() == "Cannot add duplicated Exhibit.")
                    {
                        yield return new CodeInstruction(OpCodes.Pop);
                    }
                    else
                    {
                        yield return ci;
                    }
                    prevprevCi = prevCi;
                    prevCi = ci;
                }
            }
        }







    }
}
