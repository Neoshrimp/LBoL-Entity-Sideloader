using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core.Cards;
using LBoLEntitySideloader;
using LBoLEntitySideloader.Attributes;
using LBoLEntitySideloader.Entities;
using LBoLEntitySideloader.Resource;
using System;
using System.Collections.Generic;
using System.Text;

namespace FirstCard
{
    public sealed class FirstCardDef : CardTemplate
    {
        public override IdContainer GetId()
        {
            return nameof(FirstCard);   
        }
        
        public override CardImages LoadCardImages()
        {
            var imgs = new CardImages(BepinexPlugin.embeddedSource);
            imgs.AutoLoad(this, extension: ".png");
            return imgs;
        }

        public override LocalizationOption LoadLocalization()
        {
            var loc = new GlobalLocalization(BepinexPlugin.embeddedSource);
            loc.LocalizationFiles.AddLocaleFile(LBoL.Core.Locale.En, "CardsEn.yaml");
            return loc;
        }

        public override CardConfig MakeConfig()
        {
            var cardConfig = new CardConfig(
              Index: BepinexPlugin.sequenceTable.Next(typeof(CardConfig)),
              Id: "",
              Order: 10,
              AutoPerform: true,
              Perform: new string[0][],
              GunName: "Simple1",
              GunNameBurst: "Simple1",
              DebugLevel: 0,
              Revealable: false,
              IsPooled: true,
              HideMesuem: false,
              IsUpgradable: true,
              Rarity: Rarity.Common,
              Type: CardType.Attack,
              TargetType: TargetType.SingleEnemy,
              Colors: new List<ManaColor>() { ManaColor.Red, ManaColor.White },
              IsXCost: false,
              Cost: new ManaGroup() { Red = 1, White = 1 },
              UpgradedCost: null,
              MoneyCost: null,
              Damage: 7,
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

              Keywords: Keyword.None,
              UpgradedKeywords: Keyword.None,
              EmptyDescription: false,
              RelativeKeyword: Keyword.None,
              UpgradedRelativeKeyword: Keyword.None,

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

        [EntityLogic(typeof(FirstCardDef))]
        public sealed class FirstCard : Card
        {
            protected override void SetGuns()
            {
                if (this.IsUpgraded)
                {
                    CardGuns = new Guns(new string[]
                    {
                    Config.GunNameBurst,
                    Config.GunName,
                    Config.GunNameBurst,
                    Config.GunName,
                    });
                    return;
                }
                CardGuns = new Guns(new string[]
                {
                    Config.GunName,
                    Config.GunNameBurst,
                    Config.GunName,

                });
            }
        }



    }

}
