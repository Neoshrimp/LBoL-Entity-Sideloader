using HarmonyLib;
using LBoL.Core.Units;
using LBoL.Core;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Reflection;
using LBoLEntitySideloader.ReflectionHelpers;
using UnityEngine;

namespace LBoLEntitySideloader.GameFixes
{
    class AllowDuplicateExhibits 
    {

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
                    if (prevprevCi != null && prevCi != null && ci.opcode == OpCodes.Call && (MethodInfo)ci.operand == logErrorMethod && prevCi.opcode == OpCodes.Call && prevprevCi.opcode == OpCodes.Callvirt)
                    {
                        yield return new CodeInstruction(OpCodes.Pop);
                    }
                    else if (prevprevCi != null && prevCi != null && ci.opcode == OpCodes.Ret && prevCi.opcode == OpCodes.Ldc_I4_0 && prevprevCi.opcode == OpCodes.Call && (MethodInfo)prevprevCi.operand == logErrorMethod)
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
