using LBoL.Base;
using LBoL.Base.Extensions;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Cards;
using LBoL.Presentation;
using LBoLEntitySideloader.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using static LBoLEntitySideloader.BepinexPlugin;

namespace LBoLEntitySideloader.Entities
{
    public abstract class CardTemplate : EntityDefinition,
        IConfigProvider<CardConfig>,
        IGameEntityProvider<Card>,
        IResourceConsumer<CardImages>,
        IResourceConsumer<LocalizationOption>
    {


        public override Type ConfigType()
        {
            return typeof(CardConfig);
        }

        public override Type EntityType()
        {
            return typeof(Card);
        }

        public CardConfig DefaultConfig()
        {
            var cardConfig = new CardConfig(
               Index: 0,
               Id: "",
               /// default priority for reactors/handlers. priority argument on reactors/handlers can be used instead
               Order: 10,
               AutoPerform: true,
               Perform: new string[0][],
               GunName: "",
               GunNameBurst: "",
               DebugLevel: 0,
               Revealable: false,
               IsPooled: false,
               HideMesuem: false,
               IsUpgradable: false,
               Rarity: Rarity.Common,
               Type: CardType.Unknown,
               TargetType: null,
               Colors: new List<ManaColor>() { },
               IsXCost: false,
               Cost: new ManaGroup() { },
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
       
        public abstract CardConfig MakeConfig();

        public abstract CardImages LoadCardImages();

        public void Consume(CardImages cardImages)
        {

            if (cardImages == null)
                return;

            ResourcesHelper.CardImages.AlwaysAdd(UniqueId, cardImages.main);

            if (cardImages.upgrade != null)
                ResourcesHelper.CardImages.AlwaysAdd(UniqueId + CardImages.upgradeString, cardImages.upgrade);

            var subNames = CardConfig.FromId(UniqueId).SubIllustrator;
            var subNameCount = subNames.Count();

            if (subNameCount < cardImages.subs.Count)
                log.LogWarning($"{UniqueId}: more subImages than subArtists' names");



            foreach (var kv in cardImages.subs)
            {
                if (kv.Value != null)
                    ResourcesHelper.CardImages.AlwaysAdd(kv.Key, kv.Value);
            }

        }

        public abstract LocalizationOption LoadLocalization();

        public void Consume(LocalizationOption locOptions)
        {
            ProcessLocalization(locOptions, (string key, Dictionary<string, object> value) => { TypeFactory<Card>._typeLocalizers.AlwaysAdd(key, value); });

        }



    }
}
