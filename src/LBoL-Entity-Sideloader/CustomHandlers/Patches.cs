using HarmonyLib;
using LBoL.Base;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Cards;
using LBoL.Core.Units;
using LBoL.Presentation.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace LBoLEntitySideloader.CustomHandlers
{
    [HarmonyPatch]
    class GameRunController_Init_Patch
    {

        static IEnumerable<MethodBase> TargetMethods()
        {
            yield return AccessTools.Method(typeof(GameRunController), nameof(GameRunController.Create));
            yield return AccessTools.Method(typeof(GameRunController), nameof(GameRunController.Restore));
        }

        static void Postfix(GameRunController __result)
        {

            CHandlerManager.IterateHandlers(UniqueTracker.Instance.cHandlerManager.gameRunHandlers, (hh) => hh.RegisterHandler(__result));
        }
    }


    [HarmonyPatch(typeof(UiPanelBase), nameof(UiPanelBase.LeaveGameRun))]
    class UiPanelBase_LeaveGameRun_Patch
    {
        static void Prefix(UiPanel __instance)
        {
            var gr = __instance.GameRun;
            if (gr == null)
                return;

            CHandlerManager.IterateHandlers(UniqueTracker.Instance.cHandlerManager.gameRunHandlers, (hh) => hh.UnregisterHandler(gr));
        }
    }



    [HarmonyPatch]
    class BattleController_Patch
    {

        static IEnumerable<MethodBase> TargetMethods()
        {
            yield return AccessTools.DeclaredConstructor(typeof(BattleController), new Type[] { typeof(GameRunController), typeof(EnemyGroup), typeof(IEnumerable<Card>) });

        }


        static void Postfix(BattleController __instance)
        {
            CHandlerManager.IterateHandlers(UniqueTracker.Instance.cHandlerManager.battleEventHandlers, (hh) => hh.RegisterHandler(__instance));

        }
    }



    [HarmonyPatch(typeof(BattleController), nameof(BattleController.Leave))]
    class BattleController_Leave_Patch
    {
        static void Postfix(BattleController __instance)
        {
            CHandlerManager.IterateHandlers(UniqueTracker.Instance.cHandlerManager.battleEventHandlers, (hh) => hh.UnregisterHandler(__instance));

        }
    }




}
