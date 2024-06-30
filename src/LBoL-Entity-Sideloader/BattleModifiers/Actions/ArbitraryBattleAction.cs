using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Units;
using LBoLEntitySideloader.BattleModifiers;
using LBoLEntitySideloader.BattleModifiers.Args;
using System;
using System.Collections.Generic;
using System.Text;

namespace LBoLEntitySideloader.BattleModifiers.Actions
{
    public class ArbitraryBattleAction : SimpleEventBattleAction<ArbitraryBattleEventArgs>
    {

        public ArbitraryBattleAction(Unit unit, ModUnit unitAction)
        {
            Args = new ArbitraryBattleEventArgs();
            Args.Target = unit;
            Args.unitAction = unitAction;
        }


        protected override void MainPhase()
        {
            try
            {
                Args.unitAction(Args.Target);
            }
            catch (Exception ex)
            {
                BepinexPlugin.log.LogError($"Exception during {GetType().Name}: {ex}");
            }
        }
    }
}
