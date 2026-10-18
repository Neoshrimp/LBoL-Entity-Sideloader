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
        // Our replacement method that intercepts the call
        private static Sprite CustomTryGetIntention(string text, string suffix, IntentionWidget widget)
        {
            string resolvedSuffix = suffix;

            var intention = widget._intention;
            if (intention != null && UniqueTracker.Instance.IntentionSuffixFuncs.TryGetValue(intention.Id, out var suffixFunc))
            {
                try
                {
                    resolvedSuffix = suffixFunc(intention);
                }
                catch (Exception e)
                {
                    BepinexPlugin.log.LogError($"Error invoking {nameof(IntentionTemplate.SelectAltIconsSuffix)} with {intention.GetType().Name}: {e}");
                }
            }

            return ResourcesHelper.TryGetIntention(text, resolvedSuffix);
        }

        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var targetMethod = AccessTools.Method(
                typeof(ResourcesHelper),
                nameof(ResourcesHelper.TryGetIntention),
                new[] { typeof(string), typeof(string) }
            );

            var customMethod = AccessTools.Method(
                typeof(IntentionWidgetPatch),
                nameof(CustomTryGetIntention)
            );

            // Cursor of the transpiler goes to the resource helper call
            var matcher = new CodeMatcher(instructions)
                .MatchForward(false, new CodeMatch(OpCodes.Call, targetMethod));

            if (matcher.IsInvalid)
            {
                BepinexPlugin.log.LogError("Could not find call to ResourcesHelper.TryGetIntention in IntentionWidget.UpdateProperties");
                return instructions;
            }

            // Insert 'ldarg.0' (the widget 'this' argument) right before the Call
            // Change the Call to point to our CustomTryGetIntention
            return matcher
                .InsertAndAdvance(new CodeInstruction(OpCodes.Ldarg_0)) // Inserts at the call (pushing it), then moves Pos back onto the pushed Call
                .SetOperandAndAdvance(customMethod)                    // Changes Call to CustomTryGetIntention
                .InstructionEnumeration();
        }
    }
}
