using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Battle;
using LBoL.Core;
using LBoL.Core.Cards;
using LBoL.Core.Units;
using LBoLEntitySideloader;
using LBoLEntitySideloader.Attributes;
using LBoLEntitySideloader.Entities;
using LBoLEntitySideloader.Resources;
using System;
using System.Collections.Generic;
using System.Text;
using static GunToolCard.Plugin;
using System.Reflection;

namespace GunToolCard
{
    public sealed class GunCardDefinition : CardTemplate
    {
        public override IdContainer GetId()
        {
            return nameof(ItsOverTool);
        }

        public override CardImages Load()
        {
            return new CardImages(ResourceLoader.LoadTexture(GetId(), manifestSource));
        }

        public override CardConfig MakeConfig()
        {
             var cardConfig = new CardConfig(
               Index: sequenceTable.Next(typeof(CardConfig)),
               Id: GetId(),
               Order: 10,
               AutoPerform: false,
               Perform: new string[0][],
               GunName: "Simple1",
               GunNameBurst: "Simple1",
               DebugLevel: 0,
               Revealable: false,
               IsPooled: true,
               IsUpgradable: false,
               Rarity: Rarity.Rare,
               Type: CardType.Tool,
               TargetType: TargetType.SingleEnemy,
               Colors: new List<ManaColor>() {  },
               IsXCost: false,
               Cost: new ManaGroup() { Any = 1 },
               UpgradedCost: null,
               MoneyCost: null,
               Damage: null,
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
               ToolPlayableTimes: 1,
               Keywords: Keyword.Exile | Keyword.Replenish,
               UpgradedKeywords: default,
               EmptyDescription: false,
               RelativeKeyword: default,
               UpgradedRelativeKeyword: default,

               RelativeEffects: new List<string>() { },
               UpgradedRelativeEffects: new List<string>() { },
               RelativeCards: new List<string>() { },
               UpgradedRelativeCards: new List<string>() { },
               Owner: null,
               Unfinished: false,
               Illustrator: null,
               SubIllustrator: new List<string>() { }
            );

            return cardConfig;
        }

        [EntityLogic(typeof(GunCardDefinition))]
        public sealed class ItsOverTool : Card
        {
            protected override IEnumerable<BattleAction> Actions(UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
            {
                EnemyUnit selectedEnemy = selector.SelectedEnemy;
                yield return new ForceKillAction(base.Battle.Player, selectedEnemy);
                yield break;
            }
        }
    }
}
