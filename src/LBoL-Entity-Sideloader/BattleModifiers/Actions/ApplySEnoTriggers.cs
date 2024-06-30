using LBoL.Core.Battle.BattleActions;
using LBoL.Core.StatusEffects;
using LBoL.Core.Units;
using LBoL.EntityLib.StatusEffects.Basic;
using System;
using System.Collections.Generic;
using System.Text;
using LBoL.Core;

namespace LBoLEntitySideloader.BattleModifiers.Actions
{
    public class ApplySEnoTriggers : ApplyStatusEffectAction
    {

        public ApplySEnoTriggers(Type type, Unit target, int? level = null, int? duration = null, int? count = null, int? limit = null, float occupationTime = 0, bool startAutoDecreasing = true) : base(type, target, level, duration, count, limit, occupationTime, startAutoDecreasing)
        { }


        protected override void PreEventPhase()
        {
            //base.PreEventPhase();
        }


        protected override void PostEventPhase()
        {
            //base.PostEventPhase();      
        }
    }
}
