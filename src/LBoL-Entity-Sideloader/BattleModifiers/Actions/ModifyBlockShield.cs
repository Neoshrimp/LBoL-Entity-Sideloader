using LBoL.Core;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Units;
using LBoLEntitySideloader;
using System;
using System.Collections.Generic;
using System.Text;

namespace LBoLEntitySideloader.BattleModifiers.Actions
{
    public class ModifyBlockShield : LoseBlockShieldAction
    {
        float multiplier;

        public ModifyBlockShield(Unit target, int block, int shield, float multiplier = 1f, bool forced = true) : base(target, block, shield, forced)
        {
            this.multiplier = multiplier;
            Args.Block *= -1;
            Args.Shield *= -1;

        }

        public static int ApplyMul(int val, float mul, int min) => (int)Math.Max(val * mul, min);

        protected override void PreEventPhase()
        {
            var unit = Args.Target;
            Args.Block += unit.Block - ApplyMul(unit.Block, multiplier, 0);
            Args.Shield += unit.Shield - ApplyMul(unit.Shield, multiplier, 0);

        }

        protected override void PostEventPhase()
        { }

        public override string ExportDebugDetails()
        {

            if (Args.Type != BlockShieldType.Unspecified)
            {
                return string.Format("{0} --- {{B: {1}, S: {2}, mul: {3}, Type: {5}}} --> {6}", new object[]
                {
                    GameEventArgs.DebugString(Args.Source),
                    -Args.Block,
                    -Args.Shield,
                    Args.Type,
                    multiplier,
                    GameEventArgs.DebugString(Args.Target)
                });
            }
            return string.Format("{0} --- {{B: {1}, S: {2}, mul: {3}}} --> {4}", new object[]
            {
                GameEventArgs.DebugString(Args.Source),
                -Args.Block,
                -Args.Shield,
                multiplier,
                GameEventArgs.DebugString(Args.Target)
            });

        }
    }
}
