using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;
using HarmonyLib;
using LBoL.Core.Battle.BattleActions;

namespace LBoLEntitySideloader.GameFixes
{

    // devs fixed it
    //[HarmonyPatch(typeof(PlayCardAction), nameof(PlayCardAction.ReTargeting))]
    class ReTargeting_Patch
    {

        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var matcher = new CodeMatcher(instructions).Start();

            matcher.MatchEndForward(new CodeMatch[] { OpCodes.Ldloc_2, OpCodes.Ldc_I4_3, new CodeMatch(ci => ci.opcode == OpCodes.Bne_Un_S || ci.opcode == OpCodes.Bne_Un) });


            if (matcher.IsValid)
                matcher.Set(OpCodes.Br, matcher.Instruction.operand)
                    .InsertAndAdvance(new CodeInstruction(OpCodes.Pop))
                    .InsertAndAdvance(new CodeInstruction(OpCodes.Pop));


            return matcher.InstructionEnumeration();
        }

    }




}
