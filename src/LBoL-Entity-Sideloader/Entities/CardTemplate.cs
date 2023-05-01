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


        /// <summary>
        /// Index: for index ordering in collection, library etc. Can be kept track of easily by using TemplateSequenceTable. Sideloader ensures that every index is unique although duplicate indexes are not game breaking. All modded indexes start at 12000 and can vary depending on other mods loaded.
        /// Id: fine to leave blank since it's eventually set by GetId() anyway
        /// Order: default priority for reactors/handlers. priority argument on reactors/handlers can be used instead
        /// AutoPerform: perform visual effects on cast automatically? only one card KuosanJiejie (Expanding Border) has AutoPerform set to false
        /// Perform: visual effects to perform. A bit convoluted and more experimentation is needed to fully understand it. But basically it works like this
        /// 
        /// 2do write this
        /// GunName: visual effect when the card is played. Has to be a GunConfig Id. No way to add these just yet.
        /// GunNameBurst: different (stronger) visual effect when firepower exceed half of base dmg, card is upgraded or player is in Burst
        /// DebugLevel: is the card being developed? Should always be 0 for card to appear in run.
        /// Revealable: can card be revealed in collection? Should most likely be false. This is a bit unintuitive but cards 
        /// IsPooled: can card be found naturally as reward, in shops, card gen? Cards like Yin-Yan orbs, Astrology and so on have this property set to false.
        /// HideMesuem: hide card in the collection?
        /// IsUpgradable: is card upgradable?
        /// Rarity: rarity
        /// Type: card type
        /// TargetType: what is this card targeting?
        /// Colors: colours of a card, i.e., new List<ManaColor>() { ManaColor.Red, ManaColor.Blue, ManaColor.White } would make the card Red, Blue and White. The colour is important for pooling rules. In the example, the card could only be found if the player has permanent source of Red, Blue and White mana regardless of the card's cost. If the list is left empty (Colors: new List<ManaColor>() {}) the card won't have any colour, for example, Tools don't have any color. This should not be confused with ManaColor.Colorless. Card having ManaColor.Colorless colour means that the card cannot be found unless the player has source of Colorless mana i.e. Magic Mallet. Filthless World would be example of a Colorless card. Technically, it's ok for a card to be Colorless and have some other colors like List<ManaColor>() {ManaColor.Colorless, ManaColor.Red}. This would require a source of Red and Colorless mana for the card to be pooled. ManaColor options ManaColor.Any and ManaColor.Philosophy should never be used here and will result in error used.
        /// IsXCost: does have X as mana cost option
        /// Cost: mana cost, i.e., new ManaGroup() { Any=2, Red = 1, Blue = 2 } is 2RUU
        /// UpgradedCost: if upgrade changes the mana cost specify it here. If upgrade doesn't affect cost leave null
        /// MoneyCost: how much money does this card cost to play?
        /// Damage: dmg value if the card deals damage. Else leave null.
        /// UpgradedDamage: upgraded damage value. If upgrade doesn't change damage leave null.
        /// Block: block value if card grants block. Else leave null.
        /// UpgradedBlock: upgraded block value. If upgrade doesn't change block leave null.
        /// Shield: barrier value if card grants block. Else leave null.
        /// UpgradedShield: upgraded barrier value. If upgrade doesn't change block leave null.
        /// Value1: some value which card might use for its effect. For example, Impatience uses this value to set the amount of cards it draws.
        /// UpgradedValue1: upgraded Value1 if upgrade affects it. Else null.
        /// Value2: additional optional value, same as Value1
        /// UpgradedValue2: upgraded Value2
        /// Mana: some mana group a card might use for its effect. It could, for example, be mana granted like Celestial Flight cost change like 
        /// UpgradedMana: upgraded Mana if upgrade affects it.
        /// Scry: scry amount if the card scrys.
        /// UpgradedScry: upgraded scry.
        /// ToolPlayableTimes: number times of a Limited card can be played. Referred to as {DeckCounter} in card description.
        /// Keywords: Keyword.None,
        /// UpgradedKeywords: Keyword.None,
        /// EmptyDescription: false,
        /// RelativeKeyword: Keyword.None,
        /// UpgradedRelativeKeyword: Keyword.None,
        /// RelativeEffects: new List<string>() { },
        /// UpgradedRelativeEffects: new List<string>() { },
        /// RelativeCards: new List<string>() { },
        /// UpgradedRelativeCards: new List<string>() { },
        /// Owner: null,
        /// Unfinished: false,
        /// Illustrator: null,
        /// SubIllustrator: new List<string>() { }
        /// </summary>
        /// <returns></returns>
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
