using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.StatusEffects;
using LBoL.Core.Units;
using LBoL.EntityLib.EnemyUnits.Normal.Bats;
using LBoL.Presentation;
using LBoLEntitySideloader.CustomHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static PatchWorkshop.BepinexPlugin;

namespace PatchWorkshop
{
    public class CustomHandlers
    {
        public static void LogCardName(CardsEventArgs args) => log.LogInfo($"added {args.Cards.FirstOrDefault()}");

        public static void RegisterAddFpOnDmg()
        {
            // event is registered at the start of the battle if filter condition is met
            CHandlerManager.RegisterBattleEventHandler(
                (BattleController b) => b.BattleStarting,
                AddFpOnDmgMod,
                (BattleController b) => b.AllAliveEnemies.FirstOrDefault(e => e is BatOrigin) != null,
                GameEventPriority.Highest
            );

            // in case enemy is summoned in a battle containing at least one bat in the innitial enemy group
            CHandlerManager.RegisterBattleEventHandler(
                (BattleController b) => b.EnemySpawned,
                (UnitEventArgs args) => AddFpOnDmgReactor(args.Unit),
                (BattleController b) => b.AllAliveEnemies.FirstOrDefault(e => e is BatOrigin) != null,
                GameEventPriority.Highest
            );
        }

        static void AddFpOnDmgMod(GameEventArgs args)
        {
            // battle is never null
            var battle = GameMaster.Instance.CurrentGameRun.Battle;
            foreach (var u in battle.AllAliveUnits)
            {
                AddFpOnDmgReactor(u);
            }
        }

        static void AddFpOnDmgReactor(Unit unit)
        {
            unit.ReactBattleEvent(unit.DamageDealt, (DamageEventArgs args) =>
            {
                var rez = new List<BattleAction>();
                if (args.DamageInfo.DamageType == LBoL.Base.DamageType.Attack && args.DamageInfo.Amount > 0)
                    rez.Add(new ApplyStatusEffectAction<Firepower>(args.Source, 1));
                return rez;
            });
        }
    }
}
