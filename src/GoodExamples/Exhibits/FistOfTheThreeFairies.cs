using LBoL.ConfigData;
using LBoL.Core.Cards;
using LBoLEntitySideloader;
using LBoLEntitySideloader.Attributes;
using LBoLEntitySideloader.Entities;
using LBoLEntitySideloader.Resource;
using System;
using System.Collections.Generic;
using System.Text;
using static GoodExamples.BepinexPlugin;
using UnityEngine;
using LBoL.Core;
using LBoL.Base;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;

namespace GoodExamples.Exhibits
{
    public sealed class FistOfTheThreeFairiesExDefinition : ExhibitTemplate
    {
        public override IdContainer GetId()
        {
            return nameof(FistOfTheThreeFairiesEx);
        }

        public override LocalizationOption LoadLocalization()
        {
            var globalLoc = new GlobalLocalization(embeddedSource);
            globalLoc.LocalizationFiles.AddLocaleFile(Locale.En, "ExhibitsEn");

            return globalLoc;
        }

        public override ExhibitSprites LoadSprite()
        {
            var exhibitSprites = new ExhibitSprites();
            exhibitSprites.main = ResourceLoader.LoadSprite(GetId()+".png", embeddedSource);
            return exhibitSprites;
        }

        public override ExhibitConfig MakeConfig()
        {
            var exhibitConfig = new ExhibitConfig(
                Index: 0,
                Id: "",
                Order: 10,
                IsDebug: false,
                IsPooled: true,
                IsSentinel: false,
                Revealable: false,
                Appearance: AppearanceType.Anywhere,
                Owner: "",
                LosableType: ExhibitLosableType.Losable,
                Rarity: Rarity.Rare,
                Value1: null,
                Value2: null,
                Value3: null,
                Mana: null,
                BaseManaRequirement: null,
                BaseManaColor: null,
                BaseManaAmount: 0,
                HasCounter: true,
                InitialCounter: 0,
                Keywords: Keyword.None,
                RelativeEffects: new List<string>() { },
                RelativeCards: new List<string>() { new FistOfTheThreeFairiesBigAttackDefinition().UniqueId }
                );

            return exhibitConfig;
        }

        [EntityLogic(typeof(FistOfTheThreeFairiesExDefinition))]
        public sealed class FistOfTheThreeFairiesEx : Exhibit
        {
            protected override void OnEnterBattle()
            {
                ReactBattleEvent(Battle.CardUsed, new EventSequencedReactor<CardUsingEventArgs>(OnCardUsed));

            }

            private IEnumerable<BattleAction> OnCardUsed(CardUsingEventArgs args)
            {

                if (!cardTracker.Contains(args.Card.CardType))
                {
                    cardTracker.Add(args.Card.CardType);
                    Counter++;

                    if (Counter == 3)
                    {
                        Counter = 0;
                        cardTracker.Clear();
                        yield return new AddCardsToHandAction(Library.CreateCards<FistOfTheThreeFairiesBigAttack>(1));
                    }

                }
                else
                {
                    Counter = 0;
                    cardTracker.Clear();
                }



            }

            private HashSet<CardType> cardTracker = new HashSet<CardType>();
        }
    }

    public sealed class FistOfTheThreeFairiesBigAttackDefinition : CardTemplate
    {
        public override IdContainer GetId()
        {
            return nameof(FistOfTheThreeFairiesBigAttack);
        }

        public override CardImages LoadCardImages()
        {
            return null;
        }

        public override LocalizationOption LoadLocalization()
        {
            var loc = new GlobalLocalization(embeddedSource);

            loc.LocalizationFiles.AddLocaleFile(Locale.En, "CardsEn");

            return loc;
        }

        public override CardConfig MakeConfig()
        {
            var cardConfig = new CardConfig(
                   Index: sequenceTable.Next(typeof(CardConfig)),
                   Id: "",
                   Order: 10,
                   AutoPerform: true,
                   Perform: new string[0][],
                   GunName: "Simple1",
                   GunNameBurst: "Simple1",
                   DebugLevel: 0,
                   Revealable: false,
                   IsPooled: false,
                   HideMesuem: false,
                   IsUpgradable: true,
                   Rarity: default,
                   Type: CardType.Attack,
                   TargetType: TargetType.SingleEnemy,
                   Colors: new List<ManaColor>() { ManaColor.Colorless },
                   IsXCost: false,
                   Cost: new ManaGroup() { Any  = 3 },
                   UpgradedCost: null,
                   MoneyCost: null,
                   Damage: 33,
                   UpgradedDamage: null,
                   Block: null,
                   UpgradedBlock: null,
                   Shield: null,
                   UpgradedShield: null,
                   Value1: null,
                   UpgradedValue1: null,
                   Value2: null,
                   UpgradedValue2: null,
                   Mana: null,
                   UpgradedMana: null,
                   Scry: null,
                   UpgradedScry: null,
                   ToolPlayableTimes: null,

                   Keywords: Keyword.Exile,
                   UpgradedKeywords: Keyword.Exile | Keyword.Retain,
                   EmptyDescription: false,
                   RelativeKeyword: default,
                   UpgradedRelativeKeyword: default,

                   RelativeEffects: new List<string>() { },
                   UpgradedRelativeEffects: new List<string>() { },
                   RelativeCards: new List<string>() { },
                   UpgradedRelativeCards: new List<string>() { },
                   Owner: null,
                   Unfinished: false,
                   Illustrator: "chamaruk",
                   SubIllustrator: new List<string>() { }
                );

            return cardConfig;
        }

    }
    [EntityLogic(typeof(FistOfTheThreeFairiesBigAttackDefinition))]
    public sealed class FistOfTheThreeFairiesBigAttack : Card
    {
            
    }
}
