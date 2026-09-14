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
            // target delegate is declared in SelectCardPanel.OnShowing
            static IEnumerable<MethodBase> TargetMethods()
            {
                var setActive = AccessTools.Method(typeof(GameObject), nameof(GameObject.SetActive));
                var nestedTypes = typeof(SelectCardPanel).GetNestedTypes(AccessTools.allDeclared);

                foreach (var t in nestedTypes)
                {
                    foreach (var m in t.GetMethods(AccessTools.allDeclared))
                    {
                        // Only look at compiler-generated lambdas from ViewMiniSelect
                        if (!m.Name.Contains("ViewMiniSelect")) continue;

                        // Skip abstract/empty
                        if (m.GetMethodBody() == null) continue;

                        var instr = PatchProcessor.GetCurrentInstructions(m);
                        if (instr.Any(ci => ci.Calls(setActive)))
                            yield return m;
                    }
                }
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
