using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using LBoL.Presentation.UI.Panels;
using UnityEngine;
using System;

namespace LBoLEntitySideloader.GameFixes
{
    // sideloader 0.9.7840, remove this fix for now
    // patches to fix card panel 
    class CardView_Patches
    {

        // static state variable should be good enough since there's only ever one mini cardSelect panel
        static public bool miniPanelShowing = false;


        //[HarmonyPatch(typeof(SelectCardPanel), nameof(SelectCardPanel.ViewMiniSelect))]
        class ViewMiniSelect_Patch
        {

            static void Prefix()
            {
                miniPanelShowing = true;
            }
        }



        //[HarmonyPatch]
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




        //[HarmonyPatch]
        class MiniPanelFadeDelegate_Patch
        {
            // target delegate is declared in SelectCardPanel.OnShowing
            static IEnumerable<MethodBase> TargetMethods()
            {
                // pre 1.7.1 - DisplayClass64_0
                // pre 1.7.2 - DisplayClass68_0

                var nestedTypes = typeof(SelectCardPanel).GetNestedTypes(AccessTools.allDeclared);

                var targetDelegateType = nestedTypes.SingleOrDefault(t => t.Name.Contains("DisplayClass70_0"));

                // 1.7.2 this check should be w/e
                if (targetDelegateType == null)
                    targetDelegateType = nestedTypes.SingleOrDefault(t => t.Name.Contains("DisplayClass68_0"));
                if (targetDelegateType == null)
                    throw new InvalidOperationException("No target delegate type found");

                yield return AccessTools.Method(targetDelegateType, "<ViewMiniSelect>b__0");

                //yield return AccessTools.Method(typeof(SelectCardPanel).GetNestedTypes(AccessTools.allDeclared).Single(t => t.Name.Contains("DisplayClass68_0")), "<ViewMiniSelect>b__0");
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







}
