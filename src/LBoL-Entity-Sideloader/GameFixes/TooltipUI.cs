using HarmonyLib;
using LBoL.Presentation.UI.ExtraWidgets;
using LBoL.Presentation.UI.Widgets;
using LBoL.Presentation.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace LBoLEntitySideloader.GameFixes
{
    [HarmonyPatch]
    class TooltipSource_BetterFit_Patch
    {

        static IEnumerable<MethodBase> TargetMethods()
        {
            yield return AccessTools.PropertyGetter(typeof(StatusTooltipSource), nameof(TooltipSource.TooltipPositions));
            yield return AccessTools.PropertyGetter(typeof(ExhibitTooltipSource), nameof(TooltipSource.TooltipPositions));
            yield return AccessTools.PropertyGetter(typeof(UltimateSkillTooltipSource), nameof(TooltipSource.TooltipPositions));
        }

        static void Postfix(ref TooltipPosition[] __result)
        {
            __result = __result.Concat(new TooltipPosition[] {
                new TooltipPosition(TooltipDirection.Right, TooltipAlignment.Center),
                new TooltipPosition(TooltipDirection.Left, TooltipAlignment.Center),
                // semi-reasonable default if all other positions fail to fit. SE could use a better one probably
                new TooltipPosition(TooltipDirection.Bottom, TooltipAlignment.Center),
            }).ToArray();
        }
    }

}
