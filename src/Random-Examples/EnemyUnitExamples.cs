using Cysharp.Threading.Tasks;
using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Cards;
using LBoL.Core.StatusEffects;
using LBoL.Core.Units;
using LBoL.EntityLib.Cards.Neutral.Red;
using LBoL.EntityLib.EnemyUnits.Character;
using LBoL.EntityLib.Exhibits.Shining;
using LBoL.EntityLib.JadeBoxes;
using LBoL.Presentation.UI.Panels;
using LBoLEntitySideloader;
using LBoLEntitySideloader.Attributes;
using LBoLEntitySideloader.Entities;
using LBoLEntitySideloader.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
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

            [EntityLogic(typeof(CrackedLunaDef))]
            public sealed class CrackedLuna : LightFairy
            {
                public override int AttackTimes => 3;

                //LunaMoves LunaNext;

                bool doCustomMoves = false;
                LunaMoves lunaMoves = 0;

                public override IEnumerable<IEnemyMove> GetTurnMoves()
                {
                    IEnemyMove enemyMove;

                    if (doCustomMoves)
                    {
                        switch (lunaMoves)
                        {
                            case LunaMoves.Deeznuts:
                                yield return new Simple
                            default:
                                throw new ArgumentOutOfRangeException();

                        }


                        yield break;
                    }


                    switch (this.Next)
                    {
                        case LightFairy.MoveType.Shoot:
                            enemyMove = base.AttackMove(base.GetMove(0), base.Gun1, base.Damage1 + base.EnemyBattleRng.NextInt(0, base.Damage2), this.AttackTimes, false, false, false);
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
                            doCustomMoves = true;


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
}
