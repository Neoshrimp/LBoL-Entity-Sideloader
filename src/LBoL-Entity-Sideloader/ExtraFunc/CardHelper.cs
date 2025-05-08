using HarmonyLib;
using LBoL.Base;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Battle;
using LBoL.Core.Cards;
using LBoL.Core;
using LBoL.Presentation.UI.Panels;
using LBoL.Presentation.UI;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Linq;
using LBoL.Presentation;

namespace LBoLEntitySideloader.ExtraFunc
{
    public static class CardHelper
    {

        class SkipConsumeMana { public bool skip; public SkipConsumeMana(bool val) => this.skip = val; }

        static ConditionalWeakTable<UseCardAction, SkipConsumeMana> wt_skipConsumeMana = null;// new ConditionalWeakTable<UseCardAction, SkipConsumeMana>();

        [Obsolete("Use vanilla PlayCardAction instead", error: true)]
        /// <summary>
        /// Returns card action for autoplaying a card.
        /// AutoCast skips ConsumeManaAction in UseCardAction phases thus never consumes actual mana or triggers associated listeners.
        /// </summary>
        /// <param name="card"></param>
        /// <param name="unitSelector"></param>
        /// <param name="consmingMana">passes consumed mana information. Useful for X cost cards.</param>
        /// <param name="skipConsumeManaAction">probably should always be true. 
        /// If false autocast won't skip ConsumeManaAction but consumingMana will be forced to be empty</param>
        /// <returns></returns>
        public static UseCardAction AutoCastAction(Card card, UnitSelector unitSelector, ManaGroup consmingMana, bool skipConsumeManaAction = true)
        {
            throw new NotSupportedException("Don't use AutoCastAction, use vanilla PlayCardAction instead");

            if (!skipConsumeManaAction)
                consmingMana = new ManaGroup();

            // 1.7.0 assumes kicker is false
            var uca = new UseCardAction(card, unitSelector, consmingMana, false);

            if (!skipConsumeManaAction)
            {
                FakeQueueConsumingMana();
                wt_skipConsumeMana.Add(uca, new SkipConsumeMana(false));
            }
            else
            {
                wt_skipConsumeMana.Add(uca, new SkipConsumeMana(true));
            }

            return uca;
        }

        /// <summary>
        /// Queues empty entry to BattleManaPanel._consumingDequeu
        /// Should not be used unless some very specific behavior is desired.
        /// Use AutoCastAction instead.
        /// </summary>
        [Obsolete("Why are you using this", error: true)]
        public static void FakeQueueConsumingMana()
        {
            throw new NotSupportedException("Why are you using this");

            // should not be changed
            var cost = new ManaGroup() { Any = 0 };
            var manaPanel = UiManager.GetPanel<BattleManaPanel>();
            manaPanel._consumingDeque.Insert(0, new BattleManaPanel.ConsumingManaWidgets(new ConsumingMana(cost, cost), new List<LBoL.Presentation.UI.Widgets.BattleManaWidget>() { }, new List<LBoL.Presentation.UI.Widgets.BattleManaWidget>() { }));

        }


        //[HarmonyPatch]
        class UseCardAction_Patch
        {

            static Type delegateType = typeof(UseCardAction).GetNestedTypes(AccessTools.allDeclared).Single(t => t.Name.Contains("DisplayClass17_0"));

            static FieldInfo useCardAtionInstField = AccessTools.Field(delegateType, "<>4__this");

            static IEnumerable<MethodBase> TargetMethods()
            {
                yield return AccessTools.Method(delegateType, "<GetPhases>b__1");
            }

            static bool CheckArgs(UseCardAction useCardAction)
            {
                if (wt_skipConsumeMana.TryGetValue(useCardAction, out var val))
                    return val.skip;
                return false;
            }

            static void ConsumeArgs(BattleController battle, Reactor reactor, GameEntity gameEntity, ActionCause actionCause) { }

            static int AdjustMoney(int money, UseCardAction useCardAction)
            {
                // membership test is equivalent to checking if card action is autoCast
                if (wt_skipConsumeMana.TryGetValue(useCardAction, out var _))
                {
                    var currentMoney = GameMaster.Instance?.CurrentGameRun?.Money;
                    if (currentMoney != null)
                        return Math.Min(money, currentMoney.Value);
                }
                return money;
            }

            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
            {


                return new CodeMatcher(instructions, generator)
                    .End()
                    // conditionally skip ConsumeManaAction
                    .MatchBack(false, new CodeInstruction(OpCodes.Newobj, AccessTools.Constructor(typeof(ConsumeManaAction), new Type[] { typeof(ManaGroup) })))
                    .MatchForward(false, new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(BattleController), nameof(BattleController.React), new Type[] { typeof(Reactor), typeof(GameEntity), typeof(ActionCause) })))
                    .Advance(1)
                    .CreateLabel(out var consumeArgs)
                    .Insert(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(UseCardAction_Patch), nameof(ConsumeArgs))).WithLabels(consumeArgs))
                    .Advance(1)
                    .CreateLabel(out var ret)
                    .Advance(-1)
                    .Insert(new CodeInstruction(OpCodes.Br, ret))
                    .Advance(-1)
                    // load UseCardAction instance on the stack
                    .InsertAndAdvance(new CodeInstruction(OpCodes.Ldarg_0))
                    .InsertAndAdvance(new CodeInstruction(OpCodes.Ldfld, useCardAtionInstField))
                    //
                    .InsertAndAdvance(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(UseCardAction_Patch), nameof(CheckArgs))))
                    .InsertAndAdvance(new CodeInstruction(OpCodes.Ldc_I4_0))
                    .InsertAndAdvance(new CodeInstruction(OpCodes.Bne_Un, consumeArgs))
                    // prevent consuming too much money on autocast
                    .MatchBack(false, new CodeInstruction(OpCodes.Newobj, AccessTools.Constructor(typeof(ConsumeMoneyAction), new Type[] { typeof(int) })))
                    // load UseCardAction instance on the stack
                    .InsertAndAdvance(new CodeInstruction(OpCodes.Ldarg_0))
                    .InsertAndAdvance(new CodeInstruction(OpCodes.Ldfld, useCardAtionInstField))
                    //
                    .InsertAndAdvance(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(UseCardAction_Patch), nameof(AdjustMoney))))


                    .InstructionEnumeration();
            }



        }
    }
}
