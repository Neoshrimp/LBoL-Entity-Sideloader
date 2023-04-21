using HarmonyLib;
using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Cards;
using LBoL.Core.StatusEffects;
using LBoL.Core.Units;
using LBoLEntitySideloader;
using LBoLEntitySideloader.Attributes;
using LBoLEntitySideloader.Entities;
using LBoLEntitySideloader.Resources;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static GunToolCard.Plugin;
using static UnityEngine.GraphicsBuffer;

namespace GunToolCard
{
    [OverwriteVanilla]
    public sealed class SuikaBigballDefinition : CardTemplate
    {
        public override IdContainer GetId()
        {
            return nameof(LBoL.EntityLib.Cards.Neutral.Red.SuikaBigball);
        }

        public override CardImages LoadCardImages()
        {
            var cardImages = new CardImages(embeddedSource);
            cardImages.AutoLoad(this, ".png");
            return cardImages;
        }

        public override LocalizationOption LoadText()
        {
            return null;
        }

        public override CardConfig MakeConfig()
        {
            return CardConfig.FromId(GetId());
        }

        [EntityLogic(typeof(SuikaBigballDefinition))]
        public sealed class SuikaBigball : Card
        {
            public DamageInfo HalfDamage
            {
                get
                {
                    // 2do inaccurate with negative fire power
                    return this.Damage.MultiplyBy(mult);
                }
            }

            protected override void OnEnterBattle(BattleController battle)
            {
                base.OnEnterBattle(battle);
                

                ReactBattleEvent(Battle.Player.DamageDealt, new EventSequencedReactor<DamageEventArgs>(OnPlayerlDamageDealt));


            }

            private IEnumerable<BattleAction> OnPlayerlDamageDealt(DamageEventArgs args)
            {
                if (args.ActionSource == this && args.DamageInfo.DamageType == DamageType.Attack && !showaveTriggered)
                {
                    NotifyActivating();
                    log.LogDebug(args.DamageInfo.Amount);

                    var shockwaveTargets = base.Battle.EnemyGroup.Alives.Where((EnemyUnit enemy) => enemy != originalTarget).Cast<Unit>().ToList<Unit>();
                    originalTarget = null;
                    showaveTriggered = true;
                    var dmgInfo = args.DamageInfo;

                    yield return base.AttackAction(shockwaveTargets, "Instant", new DamageInfo(dmgInfo.Damage * mult, dmgInfo.DamageType, false, this.IsAccuracy));


                }
            }

            protected override IEnumerable<BattleAction> Actions(UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
            {

                yield return PerformAction.Chat(Battle.Player, "SUIKA DEEEEEZ!!!", 3f, 0.5f, 0f, true);
                originalTarget = selector.GetEnemy(base.Battle);
                showaveTriggered = false;
                yield return base.AttackAction(selector);
                yield return base.DebuffAction<FirepowerNegative>(base.Battle.Player, base.Value1, 0, 0, 0, true, 0.2f);


            }

            Unit originalTarget;
            bool showaveTriggered = false;
            float mult = 0.5f;

        }

    }
}
