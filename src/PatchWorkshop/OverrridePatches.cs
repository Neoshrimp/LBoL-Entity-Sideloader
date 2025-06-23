using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using HarmonyLib;
using LBoL.Base.Extensions;
using LBoL.Core.Units;
using LBoL.EntityLib.EnemyUnits.Character;
using MonoMod.Utils;
using static PatchWorkshop.BepinexPlugin;

namespace PatchWorkshop
{
    // always runs since always called by Unit.EnterBattle
    [HarmonyPatch(typeof(Unit), "OnEnterGameRun")]
    class Unit_Patch
    {
        static void Prefix(Unit __instance)
        {
            log.LogDebug(string.Join("->", new StackTrace().GetFrames().Select(f => (!f.GetMethod().IsDynamicMethod() ? f.GetMethod().DeclaringType.Name + "::" : "") + f.GetMethod().Name)));
            log.LogDebug("entering GR " + __instance.GetType().Name);
        }
    }

    // doesn't run on Units (usually enemies) which override OnEnterBattle
    [HarmonyPatch(typeof(Unit), "OnEnterBattle")]
    class UnitOnEnterBattle_Patch
    {
        static void Prefix(Unit __instance)
        {
            log.LogDebug(string.Join("->", new StackTrace().GetFrames().Select(f => (!f.GetMethod().IsDynamicMethod() ? f.GetMethod().DeclaringType.Name + "::" : "") + f.GetMethod().Name)));

            log.LogDebug("ENTERING Battle " + __instance.GetType().Name);
        }
    }

    // runs with Aya
    [HarmonyPatch(typeof(Aya), nameof(Aya.OnEnterBattle))]
    class Aya_Patch
    {
        static void Prefix(Aya __instance)
        {
            log.LogDebug(string.Join("->", new StackTrace().GetFrames().Select(f => (!f.GetMethod().IsDynamicMethod() ? f.GetMethod().DeclaringType.Name + "::" : "") + f.GetMethod().Name)));
            log.LogDebug("aya entering " + __instance.GetType().Name);
        }

    }

    // runs. extra test with a method which is annotated with [AggresiveInlining]
    [HarmonyPatch(typeof(MathExtensions), nameof(MathExtensions.FloorToInt), new Type[] { typeof(float)})]
    class FloorToInt_Patch
    {
        static void Prefix()
        {
            log.LogDebug("deeznuts");
        }
    }



}
