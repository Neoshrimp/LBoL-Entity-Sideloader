using HarmonyLib;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.StatusEffects;
using LBoL.Core.Units;
using LBoL.EntityLib.EnemyUnits.Character;
using LBoL.EntityLib.EnemyUnits.Character.DreamServants;
using LBoL.EntityLib.EnemyUnits.Normal;
using LBoL.EntityLib.EnemyUnits.Normal.Drones;
using LBoL.EntityLib.EnemyUnits.Normal.Guihuos;
using LBoL.EntityLib.EnemyUnits.Opponent;
using LBoL.EntityLib.Exhibits.Shining;
using LBoL.Presentation.Units;
using LBoLEntitySideloader.BattleModifiers.Actions;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using static LBoLEntitySideloader.BepinexPlugin;


namespace LBoLEntitySideloader.BattleModifiers
{
    [HarmonyPatch]
    class ApplyEnemyMods_Patch
    {

        [HarmonyPatch(typeof(Unit), nameof(Unit.EnterBattle))]
        [HarmonyPostfix]
        static void EnterBattle_Postfix(Unit __instance)
        {

            __instance.HandleBattleEvent(
                __instance.Battle.BattleStarted,
                (args) => UnitModTracker.Instance.Perform(__instance),
                priority: (GameEventPriority)999);
        }

        [HarmonyPatch(typeof(BattleController), nameof(BattleController.Spawn), new Type[] { typeof(EnemyUnit), typeof(EnemyUnit), typeof(int), typeof(bool) })]
        [HarmonyPostfix]
        static void Spawn_Postfix(EnemyUnit __result)
        {
            UnitModTracker.Instance.Perform(__result);
        }

    }


    [HarmonyPatch(typeof(GameDirector), nameof(GameDirector.EnterBattle))]
    class GameDirectorEnterBattle_Patch
    {
        static void Postfix(GameDirector __instance, BattleController battle)
        {
            battle.ActionViewer.Register(new BattleActionViewer<ApplySEnoTriggers>(__instance.ApplyStatusEffectViewer));
            battle.ActionViewer.Register(new BattleActionViewer<ModifyBlockShield>(GameDirector.LoseBlockShieldViewer));

        }
    }


    [HarmonyPatch(typeof(GameDirector), nameof(GameDirector.LeaveBattle))]
    class GameDirectorLeaveBattle_Patch
    {
        static void Postfix(GameDirector __instance, BattleController battle)
        {
            battle.ActionViewer.Unregister(new BattleActionViewer<ApplySEnoTriggers>(__instance.ApplyStatusEffectViewer));
            battle.ActionViewer.Unregister(new BattleActionViewer<ModifyBlockShield>(GameDirector.LoseBlockShieldViewer));

        }
    }



}
