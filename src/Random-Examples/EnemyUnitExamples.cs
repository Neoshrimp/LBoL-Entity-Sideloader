using Cysharp.Threading.Tasks;
using HarmonyLib;
using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Cards;
using LBoL.Core.StatusEffects;
using LBoL.Core.Units;
using LBoL.EntityLib.Cards.Neutral.Red;
using LBoL.EntityLib.Cards.Other.Enemy;
using LBoL.EntityLib.EnemyUnits.Character;
using LBoL.EntityLib.Exhibits.Shining;
using LBoL.EntityLib.JadeBoxes;
using LBoL.Presentation.UI.Panels;
using LBoLEntitySideloader;
using LBoLEntitySideloader.Attributes;
using LBoLEntitySideloader.Entities;
using LBoLEntitySideloader.ReflectionHelpers;
using LBoLEntitySideloader.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using UnityEngine;
using static Random_Examples.BepinexPlugin;


namespace Random_Examples
{
    public class EnemyUnitExamples
    {

        [OverwriteVanilla]
        public sealed class CrackedLunaDef : EnemyUnitTemplate
        {
            public override IdContainer GetId() => nameof(Luna);

            [DontOverwrite]
            public override LocalizationOption LoadLocalization()
            {
                //return new DirectLocalization(new Dictionary<string, object>() { "Moves" });
                return null;
            }


            public override EnemyUnitConfig MakeConfig()
            {
                var con = EnemyUnitConfig.FromId(GetId());
                con.Damage1 = 4;
                con.Damage1Hard = 6;
                con.Damage1Lunatic = 8;
                return con;
            }


        }


        // Sunny sets spell card rng of the start of the battle but refers to the original Luna type.
        // That needs to be changed.
        [HarmonyPatch]
        class Sunny_Patch
        {

            static IEnumerable<MethodBase> TargetMethods()
            {
                yield return AccessTools.Method(typeof(Sunny), nameof(Sunny.OnEnterBattle));
                yield return ExtraAccess.InnerMoveNext(typeof(Sunny), nameof(Sunny.OnBattleStarted));
            }


            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                foreach (var ci in instructions)
                {
                    if (ci.Is(OpCodes.Isinst, typeof(LBoL.EntityLib.EnemyUnits.Character.Luna)))
                    {
                        yield return new CodeInstruction(OpCodes.Isinst, typeof(Luna));
                    }
                    else
                    {
                        yield return ci;
                    }
                }
            }


        }




        [EntityLogic(typeof(CrackedLunaDef))]
        public sealed class Luna : LightFairy
        {
            int MultiAttack = 1;

            //LunaMoves LunaNext;

            bool doCustomMoves = false;
            LunaMoves lunaMoves = 0;


            IEnumerable<BattleAction> DeeznutsMove()
            {
                yield return PerformAction.Chat(this,
@"deeznuts deeznuts deeznuts
deeznuts deeznuts deeznuts
deeznuts deeznuts deeznuts
deeznuts deeznuts deeznuts
deeznuts deeznuts deeznuts
deeznuts deeznuts deeznuts
deeznuts deeznuts deeznuts"
, 3f
);
                yield return new ApplyStatusEffectAction<Spirit>(this, 2);
                MultiAttack += 1;
                //yield return new ApplyStatusEffectAction<Firepower>(this, 2);

            }

            // original Luna's spells
            public override IEnumerable<BattleAction> LightActions()
            {
                yield return new EnemyMoveAction(this, base.LightMove, true);
                yield return PerformAction.Animation(this, "shoot3", 0.5f, null, 0f, -1);
                yield return new AddCardsToDiscardAction(new Card[] { Library.CreateCard<Yueguang>() });
            }

            public override IEnumerable<BattleAction> SpellActions()
            {
                yield return PerformAction.Spell(this, "寂静之月");
                yield return new EnemyMoveAction(this, base.SpellCard, true);
                bool anime = true;
                foreach (EnemyUnit enemy in base.AllAliveEnemies)
                {
                    Unit unit = enemy;
                    int? num = new int?(base.Count1);
                    bool flag = enemy.RootIndex <= base.RootIndex;
                    yield return new ApplyStatusEffectAction<Graze>(unit, num, null, null, null, 0f, flag);
                    yield return new CastBlockShieldAction(this, enemy, 0, base.Defend, BlockShieldType.Normal, anime);
                    anime = false;
                }
                yield return PerformAction.Chat(this, "Chat.LunaSpell".Localize(true), 2.5f, 0.2f, 0f, true);
            }

            public override IEnumerable<IEnemyMove> GetTurnMoves()
            {
                IEnemyMove enemyMove;

                if (doCustomMoves)
                {
                    switch (lunaMoves)
                    {
                        case LunaMoves.Deeznuts:
                            yield return new SimpleEnemyMove(Intention.Unknown(), DeeznutsMove());
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();

                    }


                    yield break;
                }


                switch (this.Next)
                {
                    case LightFairy.MoveType.Shoot:
                        enemyMove = base.AttackMove(base.GetMove(0), base.Gun1, base.Damage1 + base.EnemyBattleRng.NextInt(0, base.Damage2), MultiAttack, false, false, false);
                        break;
                    case LightFairy.MoveType.Light:
                        enemyMove = new SimpleEnemyMove(Intention.AddCard(), this.LightActions());
                        break;
                    case LightFairy.MoveType.Spell:
                        enemyMove = new SimpleEnemyMove(Intention.SpellCard(this.SpellCard, null, null, false), this.SpellActions());
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                yield return enemyMove;
            }


            public override void UpdateMoveCounters()
            {
                MoveType moveType = 0;

                if (!doCustomMoves && Next == MoveType.Light)
                {
                    // 50% roll
                    if (EnemyMoveRng.Next(1) == 1)
                    {
                        doCustomMoves = true;
                    }

                    return;
                }
                if (doCustomMoves)
                {
                    doCustomMoves = false;
                    Next = MoveType.Spell;
                    return;
                }


                switch (Next)
                {
                    case MoveType.Shoot:
                        moveType = MoveType.Light;
                        break;
                    case MoveType.Light:
                        moveType = MoveType.Shoot;
                        break;
                    case MoveType.Spell:
                        moveType = MoveType.Shoot;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                this.Next = moveType;
            }


            public enum LunaMoves
            {
                Deeznuts
            }


        }
    }
}
