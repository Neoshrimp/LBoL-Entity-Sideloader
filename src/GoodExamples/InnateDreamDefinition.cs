using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Battle;
using LBoL.Core;
using LBoL.Core.Cards;
using LBoLEntitySideloader;
using LBoLEntitySideloader.Attributes;
using LBoLEntitySideloader.Entities;
using LBoLEntitySideloader.Resource;
using System;
using System.Collections.Generic;
using System.Text;
using static GoodExamples.BepinexPlugin;
using LBoL.Core.StatusEffects;
using UnityEngine;

namespace GoodExamples
{
    public sealed class InnateDreamDefinition : CardTemplate
    {
        public override IdContainer GetId()
        {
            return nameof(InnateDream);
        }

        public override CardImages LoadCardImages()
        {
            var imgs = new CardImages(embeddedSource);
            imgs.AutoLoad(this, ".png");
            return imgs;
        }

        public override LocalizationOption LoadLocalization()
        {
            return new GlobalLocalization(embeddedSource);
        }

        public override CardConfig MakeConfig()
        {
            var cardConfig = new CardConfig(
               Index: sequenceTable.Next(typeof(CardConfig)),
               Id: "",
               Order: 10,
               AutoPerform: true,
               Perform: new string[0][],
               // some amulet shooting vfx
               GunName: "梦之御札",
               GunNameBurst: "梦之御札B",
               DebugLevel: 0,
               Revealable: true,
               IsPooled: true,
               HideMesuem: false,
               IsUpgradable: true,
               Rarity: Rarity.Rare,
               Type: CardType.Attack,
               TargetType: TargetType.SingleEnemy,
               Colors: new List<ManaColor>() { ManaColor.Red, ManaColor.White},
               IsXCost: false,
               Cost: new ManaGroup() { Any = 2, Red = 1, White = 1 },
               UpgradedCost: null,
               MoneyCost: null,
               Damage: 32,
               UpgradedDamage: 48,
               Block: null,
               UpgradedBlock: null,
               Shield: null,
               UpgradedShield: null,
               Value1: null,
               UpgradedValue1: null,
               Value2: null,
               UpgradedValue2: null,
               Mana: new ManaGroup() { Any = 2 },
               UpgradedMana: null,
               Scry: null,
               UpgradedScry: null,
               ToolPlayableTimes: null,

               Loyalty: null,
                UpgradedLoyalty: null,
                PassiveCost: null,
                UpgradedPassiveCost: null,
                ActiveCost: null,
                UpgradedActiveCost: null,
                UltimateCost: null,
                UpgradedUltimateCost: null,

               Keywords: Keyword.Ethereal,
               UpgradedKeywords: Keyword.Ethereal,
               EmptyDescription: false,
               RelativeKeyword: Keyword.TempMorph,
               UpgradedRelativeKeyword: Keyword.TempMorph,

               RelativeEffects: new List<string>() {  },
               UpgradedRelativeEffects: new List<string>() { },
               RelativeCards: new List<string>() { },
               UpgradedRelativeCards: new List<string>() { },
               Owner: VanillaCharNames.Reimu,
               Unfinished: false,
               Illustrator: "petas",
               SubIllustrator: new List<string>() { }
            );

            return cardConfig;
        }


        [EntityLogic(typeof(InnateDreamDefinition))]
        public sealed class InnateDream : Card
        {
            protected override void OnEnterBattle(BattleController battle)
            {
                ReactBattleEvent(Battle.BattleStarted, new EventSequencedReactor<GameEventArgs>(OnBattleStarted));
            }

            private IEnumerable<BattleAction> OnBattleStarted(GameEventArgs args)
            {
                if (Battle?.Player.Hp <= 30 && this.Zone != CardZone.Hand)
                {
                    NotifyActivating();

                    yield return new MoveCardAction(this, CardZone.Hand);

                    // if cost is not less than 2. roughly..
                    if (!Cost.IsSubset(Mana))
                    {
                        SetTurnCost(Mana);
                    } 
                }

            }
        }


    }
}
