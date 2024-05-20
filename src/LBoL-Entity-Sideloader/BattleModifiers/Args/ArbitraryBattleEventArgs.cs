using LBoL.Core;
using LBoL.Core.Units;
using LBoLEntitySideloader.BattleModifiers;
using System;
using System.Collections.Generic;
using System.Text;

namespace LBoLEntitySideloader.BattleModifiers.Args
{
    public class ArbitraryBattleEventArgs : GameEventArgs
    {
        public Unit Target { get; set; }
        public ModUnit unitAction { get; set; }

        protected override string GetBaseDebugString()
        {
            return $"ArbitraryAction: {DebugString(Target)}";
        }
    }
}
