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
using LBoL.Base.Extensions;
using System.Collections;
using LBoL.Presentation;
using LBoL.EntityLib.Cards.Neutral.Blue;

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
            // embedded resource folders are separated by a dot
            var folder = "FistOfTheFreeFairies.";
            var exhibitSprites = new ExhibitSprites();

            Func<string, Sprite> wrap = (s) => ResourceLoader.LoadSprite((folder + GetId() + s + ".png"), embeddedSource);

            exhibitSprites.main = wrap("");


            exhibitSprites.customSprites.Add("none", wrap("_none"));
            exhibitSprites.customSprites.Add("luna", wrap("_luna"));
            exhibitSprites.customSprites.Add("-luna", wrap("_-luna"));
            exhibitSprites.customSprites.Add("star", wrap("_star"));
            exhibitSprites.customSprites.Add("-star", wrap("_-star"));
            exhibitSprites.customSprites.Add("sunny", wrap("_sunny"));
            exhibitSprites.customSprites.Add("-sunny", wrap("_-sunny"));


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

            public override string OverrideIconName
            {
                get
                {
                    if (Battle == null)
                        return Id;

                    if (triggered)
                        return Id;

                    if (cardTracker.Empty())
                        return Id + "none";
                    if (cardTracker.Count == 1)
                    { 
                        if(cardTracker.Contains(CardType.Attack))
                            return Id + "sunny";
                        if (cardTracker.Contains(CardType.Defense))
                            return Id + "star";
                        if (cardTracker.Contains(CardType.Skill))
                            return Id + "luna";
                    }
                    if (cardTracker.Count == 2)
                    {
                        if (!cardTracker.Contains(CardType.Attack))
                            return Id + "-sunny";
                        if (!cardTracker.Contains(CardType.Defense))
                            return Id + "-star";
                        if (!cardTracker.Contains(CardType.Skill))
                            return Id + "-luna";
                    }


                    return Id;
                }
            
            }



            protected override void OnEnterBattle()
            {
                ReactBattleEvent(Battle.CardUsed, new EventSequencedReactor<CardUsingEventArgs>(OnCardUsed));
                Counter = 0;
                cardTracker.Clear();
                // notify changed raises event to check if the icon needs changing (essentially, makes ExhibitWidget call OverideIconName). It's purely for visual effects.
                NotifyChanged();
            }

            protected override void OnLeaveBattle()
            {
                base.OnLeaveBattle();
                Counter = 0;
                cardTracker.Clear();
                NotifyChanged();
            }

            private IEnumerable<BattleAction> OnCardUsed(CardUsingEventArgs args)
            {

                var cardType = args.Card.CardType;


                if ((CardType.Attack | CardType.Defense | CardType.Defense).HasFlag(cardType))
                {

                    if (!cardTracker.Contains(cardType))
                    {
                        cardTracker.Add(cardType);
                        Counter++;

                        if (Counter == 3)
                        {
                            NotifyActivating();
                            Counter = 0;
                            cardTracker.Clear();
                            triggered = true;
                            GameMaster.Instance.StartCoroutine(ResetTrigger());
                            yield return new AddCardsToHandAction(Library.CreateCards<FistOfTheThreeFairiesBigAttack>(1));
                        }
                    }
                    else
                    {
                        Counter = 1;
                        cardTracker.Clear();
                        cardTracker.Add(cardType);
                    }

                }
                else
                {
                    Counter = 0;
                    cardTracker.Clear();
                }

            }


            IEnumerator ResetTrigger()
            {
                yield return new WaitForSeconds(2);
                triggered = false;
                NotifyChanged();
            }
            

            private HashSet<CardType> cardTracker = new HashSet<CardType>();

            private bool triggered;
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
                   // perform CameraFlash effect, player unit does spellcard animation, play NuclearBomb sfx
                   Perform: new string[3][] { new string []{ "2", "CameraFlash" }, new string[] { "3", "spell" }, new string[] { "4", "Nuclearbomb" } },
                   // SuikaBigball gun
                   GunName: "元鬼玉",
                   GunNameBurst: "元鬼玉B",
                   DebugLevel: 0,
                   Revealable: false,
                   IsPooled: false,
                   HideMesuem: false,
                   IsUpgradable: true,
                   Rarity: default,
                   Type: CardType.Attack,
                   TargetType: TargetType.SingleEnemy,
                   Colors: new List<ManaColor>() { ManaColor.Red, ManaColor.Blue, ManaColor.White },
                   IsXCost: false,
                   Cost: new ManaGroup() { Any  = 3 },
                   UpgradedCost: new ManaGroup() { Any = 3 },
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

                   Keywords: Keyword.Exile | Keyword.TempRetain,
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
                   Illustrator: "@chamaruk",
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
