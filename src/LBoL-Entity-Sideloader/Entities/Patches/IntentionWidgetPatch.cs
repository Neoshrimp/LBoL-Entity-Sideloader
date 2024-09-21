using HarmonyLib;
using LBoL.Core;
using LBoL.Core.Units;
using LBoL.EntityLib.EnemyUnits.Character;
using LBoL.Presentation;
using LBoL.Presentation.UI.Widgets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using UnityEngine;


namespace LBoLEntitySideloader.Entities.Patches
{
    [HarmonyPatch(typeof(IntentionWidget), nameof(IntentionWidget.UpdateProperties))]
    class IntentionWidgetPatch
    {

        private static void ChangeSuffix(Intention intention, ref string suffix)
        {
            if (!UniqueTracker.Instance.IntentionSuffixFuncs.TryGetValue(intention.Id, out var suffixFunc))
                return;

            try
            {
                suffix = suffixFunc(intention);
            }
            catch (Exception e)
            {
                BepinexPlugin.log.LogError($"Error invoking {nameof(IntentionTemplate.SelectAltIconsSuffix)} with {intention.GetType().Name}: {e}");
            }

        }

        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {


            return new CodeMatcher(instructions)
            .MatchEndForward(new CodeInstruction(OpCodes.Stloc_1))
            .InsertAndAdvance(new CodeInstruction(OpCodes.Ldarg_0))
            .InsertAndAdvance(new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(IntentionWidget), nameof(IntentionWidget._intention))))
            .InsertAndAdvance(new CodeInstruction(OpCodes.Ldloca, 0))
            .InsertAndAdvance(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(IntentionWidgetPatch), nameof(IntentionWidgetPatch.ChangeSuffix))))

            .InstructionEnumeration();
        }

    }
}
