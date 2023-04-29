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
using LBoLEntitySideloader.Resource;
using System;
using System.Collections.Generic;
using System.Text;
using static GunToolCard.Plugin;
using System.Reflection;
using YamlDotNet.RepresentationModel;
using HarmonyLib;
using LBoL.Core.StatusEffects;

namespace GunToolCard
{
    public sealed class GlockToolDefinition : CardTemplate
    {
        public override IdContainer GetId()
        {
            return nameof(GlockTool);
        }

        public override CardImages LoadCardImages()
        {
            var cardImages = new CardImages(embeddedSource);
            // requires following naming conventions to work properly
            cardImages.AutoLoad(this, ".png");

            return cardImages;
        }

        public override LocalizationOption LoadLocalization()
        {

            var locFiles = new LocalizationFiles(embeddedSource);

            //locFiles.AddLocaleFile(Locale.En, GetId() + "En");

            //locFiles.AddLocaleFile(Locale.ZhHans, GetId() + "ZhHans");


            locFiles.AddLocaleFile(Locale.En, "CardsEn");


            locFiles.AddLocaleFile(Locale.ZhHans, "CardsZhHans");


            var globalLoc = new GlobalLocalization(locFiles);


            return globalLoc;
        }

        public override CardConfig MakeConfig()
        {
             var cardConfig = new CardConfig(
               Index: sequenceTable.Next(typeof(CardConfig)),
               Id: GetId(),
               Order: 10,
               AutoPerform: true,
               Perform: new string[0][],
               GunName: "Simple1",
               GunNameBurst: "Simple1",
               DebugLevel: 0,
               Revealable: false,
               IsPooled: true,
               HideMesuem: false,
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
               Keywords: Keyword.Exile,
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
               Illustrator: "@abikozyozi",
               SubIllustrator: new List<string>() { "alt1" }
            );

            return cardConfig;
        }


        [EntityLogic(typeof(GlockToolDefinition))]
        public sealed class GlockTool : Card
        {
            protected override IEnumerable<BattleAction> Actions(UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
            {
                EnemyUnit selectedEnemy = selector.SelectedEnemy;
                yield return new ForceKillAction(base.Battle.Player, selectedEnemy);
            }
        }



    }
}
