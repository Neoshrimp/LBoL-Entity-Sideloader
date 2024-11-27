using HarmonyLib;
using LBoL.Base.Extensions;
using LBoL.Core.Battle;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

namespace LBoLEntitySideloader.GameFixes
{
    [HarmonyPatch(typeof(BattleController), nameof(BattleController.CalculateBlockShield))]
    class CalculateBlockShield_RoundingFix_Patch
    {

        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var matcher = new CodeMatcher(instructions).Start();

            while (matcher.IsValid)
            {
                matcher.MatchEndForward(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(MathExtensions), nameof(MathExtensions.RoundToInt), new Type[] { typeof(float) })));
                if (matcher.IsInvalid)
                    break;

                matcher.SetInstruction(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(MathExtensions), nameof(MathExtensions.RoundToInt), new Type[] { typeof(float), typeof(MidpointRounding) })))
                    .InsertAndAdvance(new CodeInstruction(OpCodes.Ldc_I4_1));
            }

            return matcher.InstructionEnumeration();
        }

    }
}
