using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.StatusEffects;
using LBoL.Core.Units;
using LBoL.EntityLib.Exhibits.Shining;
using LBoL.EntityLib.StatusEffects.Basic;
using LBoL.Presentation;
using LBoL.Presentation.UI;
using LBoL.Presentation.UI.Panels;
using LBoL.Presentation.Units;
using LBoLEntitySideloader.BattleModifiers.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace LBoLEntitySideloader.BattleModifiers
{
    public class ModFactory
    {
        /// <summary>
        /// Avoids freezing the argument during early initialization.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="getArg"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static ModUnit LazyArg<T>(Func<T> getArg, Func<T, ModUnit> target) => (unit) =>
        {
            var arg = getArg();
            return target(arg)(unit);
        };


        public static ModUnit LazyArg<T, T1>(Func<T> getArg, Func<T1> getArg2, Func<T, T1, ModUnit> target) => (unit) =>
        {
            var arg = getArg();
            var arg2 = getArg2();
            return target(arg, arg2)(unit);
        };

        public static ModUnit LazyArg<T, T1, T2>(Func<T> getArg, Func<T1> getArg2, Func<T2> getArg3, Func<T, T1, T2, ModUnit> target) => (unit) =>
        {
            var arg = getArg();
            var arg2 = getArg2();
            var arg3 = getArg3();
            return target(arg, arg2, arg3)(unit);
        };

        static public ModUnit AddSE<T>(int? level = null, int? duration = null, int? count = null, int? limit = null, float occupationTime = 0f, bool startAutoDecreasing = true) where T : StatusEffect
            => AddSE(typeof(T), level, duration, count, limit, occupationTime, startAutoDecreasing);

        static public ModUnit AddSE(Type se, int? level = null, int? duration = null, int? count = null, int? limit = null, float occupationTime = 0f, bool startAutoDecreasing = true)
        {
            return (unit) =>
            {
                unit.React(new BattleAction[] { new ApplySEnoTriggers(type: se, target: unit, level: level, duration: duration, count: count, limit: limit, occupationTime: occupationTime, startAutoDecreasing: startAutoDecreasing) });

                return unit;
            };
        }


        static public ModUnit MulEffetiveHp(float multiplier)
        {
            return MultiplyHp(multiplier, multiplyBlock: true);
        }

        static public ModUnit MultiplyHp(float multiplier, bool multiplyBlock = false)
        {
            return (unit) =>
            {

                var hp = ModifyBlockShield.ApplyMul(unit.MaxHp, multiplier, 1);
                unit.SetMaxHp(hp, hp);

                if (multiplyBlock)
                    unit.React(new ModifyBlockShield(unit, 0, 0, multiplier, forced: true));

                var unitView = unit.View as UnitView;
                if (unitView != null)
                {
                    unitView._statusWidget.SetHpBar();
                    if (unit is PlayerUnit)
                    {
                        UiManager.GetPanel<SystemBoard>().OnMaxHpChanged();
                        unitView.OnMaxHpChanged();
                    }
                }
                return unit;
            };
        }

        // 2do doesn't work on spawns
        static public ModUnit ScaleModel(float multiplier)
        {
            return (unit) =>
            {
                var unitView = unit.View as UnitView;
                if (unitView != null)
                {
                    unitView.transform.localScale = unitView.transform.localScale * multiplier;
                }
                return unit;
            };
        }

        static public ModUnit StageCheck(Type stageType, ModUnit mod)
        {
            return (unit) => { if (PrecondFactory.IsStage(stageType)(unit)) return unit; return mod(unit); };
        }

        static public ModUnit DoSomeAction(ModUnit action)
        {
            return (unit) => { unit.React(new ArbitraryBattleAction(unit, action)); return unit; };
        }


    }
}
