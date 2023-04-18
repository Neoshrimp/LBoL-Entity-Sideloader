using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Battle;
using LBoL.Core;
using LBoL.Core.Cards;
using LBoLEntitySideloader;
using LBoLEntitySideloader.Attributes;
using LBoLEntitySideloader.Entities;
using LBoLEntitySideloader.Resources;
using System;
using System.Collections.Generic;
using System.Text;
using YamlDotNet.RepresentationModel;
using static GunToolCard.Plugin;


namespace GunToolCard
{
    public sealed class InnateDreamDefinition : CardTemplate
    {
        public override IdContainer GetId()
        {
            return nameof(InnateDream);
        }

        public override CardImages LoadCardImages()
        {
            return null;
        }

        public override YamlMappingNode LoadYaml()
        {
            return ResourceLoader.LoadYaml(GetId() + ".yaml", embeddedSource);

        }

        public override CardConfig MakeConfig()
        {
            var cardConfig = new CardConfig(
               Index: sequenceTable.Next(typeof(CardConfig)),
               Id: "",
               Order: 10,
               AutoPerform: true,
               Perform: new string[0][],
               GunName: "TSakuyaKnife",
               GunNameBurst: "TSakuyaKnifeB",
               DebugLevel: 0,
               Revealable: true,
               IsPooled: true,
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

               Keywords: Keyword.Ethereal,
               UpgradedKeywords: Keyword.Ethereal,
               EmptyDescription: false,
               RelativeKeyword: Keyword.TempMorph,
               UpgradedRelativeKeyword: Keyword.TempMorph,

               RelativeEffects: new List<string>() {  },
               UpgradedRelativeEffects: new List<string>() { },
               RelativeCards: new List<string>() { },
               UpgradedRelativeCards: new List<string>() { },
               Owner: "Reimu",
               Unfinished: false,
               Illustrator: null,
               SubIllustrator: new List<string>() { }
            );

            return cardConfig;
        }


        [EntityLogic(typeof(InnateDreamDefinition))]
        public sealed class InnateDream : Card
        {
            protected override void OnEnterBattle(BattleController battle)
            {
                base.ReactBattleEvent<GameEventArgs>(base.Battle.BattleStarted, new EventSequencedReactor<GameEventArgs>(this.OnBattleStarted));
            }

            private IEnumerable<BattleAction> OnBattleStarted(GameEventArgs args)
            {


                if (Battle?.Player.Hp <= 30)
                {

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
