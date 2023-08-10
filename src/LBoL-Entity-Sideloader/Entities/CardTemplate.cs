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


        public static class VanillaCharNames
        {
            public const string Reimu = "Reimu";
            public const string Marisa = "Marisa";
            public const string Sakuya = "Sakuya";
            public const string Cirno = "Cirno";
            public const string Koishi = "Koishi";
        }

        /// <summary>
        /// Common default values for CardConfig. The values which are safe to be left as a null are left as null.
        /// </summary>
        /// <returns></returns>
        public CardConfig DefaultConfig()
        {

            var cardConfig = new CardConfig(
               Index: 0,
               Id: "",
               Order: 10,
               AutoPerform: true,
               Perform: new string[0][],
               GunName: "",
               GunNameBurst: "",
               DebugLevel: 0,
               Revealable: false,
               IsPooled: false,
               HideMesuem: false,
               IsUpgradable: true,
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

               Loyalty: null,
               UpgradedLoyalty: null,
               PassiveCost : null,
               UpgradedPassiveCost : null,
               ActiveCost : null,
               UpgradedActiveCost : null,
               UltimateCost : null,
               UpgradedUltimateCost : null,

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


        /// <summary>
        /// Index: for index ordering in collection, library etc. Can be kept track of easily by using TemplateSequenceTable. Sideloader ensures that every index is unique although duplicate indexes are not game breaking. All modded indexes start at 12000 and can vary depending on other mods loaded.
        /// Id: should be left blank since it's eventually set by GetId() anyway.
        /// Order: default priority for reactors/handlers. Priority is used to set order of effects which trigger at the same time, for example, TurnStarted. Priority argument on reactors/handlers can be used instead.
        /// AutoPerform: perform visual effects on cast automatically? Only one card  Expanding Border(Id:KuosanJiejie) has AutoPerform set to false.
        /// Perform: visual effects to perform. A bit convoluted and experimentation is needed to fully understand how it works. It best to leave this empty and perform visual effects in Card Actions. TimeWalk is a good example how to perform various effects when card is played. 
        /// Perform needs to be an array of string arrays where each inner array contains information about effect to be performed. The first element of inner array can be "1", "2", "3" or "4". This specifies the type of effect to perform. Second element is the name of the effect. Third, is optional additional property for the effect.
        /// 
        /// "1" - Gun (attack effect). Name must be from GunConfig. 3rd optional parameter is occupationTime locking the game for certain time in seconds after the effect is performed. Can be float i.e. "1.2"
        /// "2" - Effect. Name must be from "EffectConfig". 3rd optional parameter is delay in seconds before the effect is performed (sideloader only). It can be used to chain effects one after the other. Can be float.
        /// "3" - unit animation. Player units have specific spine animations like "spell", "idle" (no full list yet). 3rd optional parameter is occupationTime.
        /// "4" - sfx. Name must be from SfxConfig.  3rd optional parameter is delay (sideloader only)
        /// 
        /// GunName: visual effect when the card is played. Has to be a GunConfig Id. No way to add these just yet.
        /// GunNameBurst: different (stronger) visual effect when firepower exceed half of base dmg, card is upgraded or player is in Burst
        /// DebugLevel: is the card being developed? Should always be 0 for card to appear in a run.
        /// Revealable: can card be revealed in collection? Should most likely be false. This is a bit unintuitive but cards with Revealable false are revealed in the collection without need to find them.
        /// IsPooled: can card be found naturally as reward, in shops, card gen? Cards like Yin-Yan orbs, Astrology and so on have this property set to false.
        /// HideMesuem: hide card in the collection?
        /// IsUpgradable: is card upgradable?
        /// Rarity: rarity
        /// Type: card type
        /// TargetType: what is this card targeting? TargetType.All is bugged, I think.
        /// Colors: colours of a card, i.e., new List<ManaColor>() { ManaColor.Red, ManaColor.Blue, ManaColor.White } would make the card Red, Blue and White. The colour is important for pooling rules. In the example, the card could only be found if the player has permanent source of Red, Blue and White mana regardless of the card's cost. If the list is left empty (Colors: new List<ManaColor>() {}) the card won't have any colour, for example, Tools don't have any color. This should not be confused with ManaColor.Colorless. Card having ManaColor.Colorless colour means that the card cannot be found unless the player has source of Colorless mana i.e. Magic Mallet. Filthless World would be an example of a Colorless card. Technically, it's ok for a card to be Colorless and have some other colors like List<ManaColor>() {ManaColor.Colorless, ManaColor.Red}. This would require a source of Red and Colorless mana for the card to be pooled. ManaColor options ManaColor.Any and ManaColor.Philosophy should never be used here and will result in error used.
        /// IsXCost: does the card have X as mana cost option
        /// Cost: mana cost, i.e., new ManaGroup() { Any=2, Red = 1, Blue = 2 } is 2RUU. Empty list = 0 cost
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
        /// Keywords: keywords which directly affect how the card works like Exile, Replenish etc. Multiple keywords can be specified by using | operator, i.e., Keyword.Replenish | Keyword.Exile | Keyword.Accuracy
        /// UpgradedKeywords: keywords after upgrading. It's important to note that unlike other upgraded properties base card variant keywords need to respecified if they are desired to be kept in the upgraded version. Some upgrades might remove or change 'bad' keywords like Exile.
        /// EmptyDescription: does the card have NO description? Some status cards don't have a description.
        /// RelativeKeyword: keywords which don't have direct effect but are relevant for what the card does like Overdraft (Overdraft is triggered by yielding LockRandomTurnManaAction action in Card Actions). They will appear as a tooltip next to the card. Note that common keywords like block, barrier, scry can be inferred and will be added to the tooltip automatically without being specified. 
        /// UpgradedRelativeKeyword: if keywords requiring explanation change or remain the same after upgrade they will need to be specified here.
        /// RelativeEffects: list of StatusEffect Ids relevant for the effect of the card like Firepower or TimeAuraSe(Timepulse)
        /// UpgradedRelativeEffects: list of StatusEffects after upgrading. Base effect might need to be respecified.
        /// RelativeCards: list of Card Ids which are created or otherwise relevant to effect of the card. Example, Queen of Yin-Yang Orb (Id:YinyangQueen).
        /// UpgradedRelativeCards: list of cards for upgraded version of the card. Base list might need to be respecified.
        /// Owner: owner of the card, one of the playable characters. Leave null to make the card Neutral. VanillaCharNames can be used to avoid typos.
        /// Unfinished: should the card have 'Unfinished' disclaimer on card image.
        /// Illustrator: name of the artist.
        /// SubIllustrator: list of subartist names. These name will be used as a key suffix when card images are cached to ResourcesHelper.CardImages dictionary.
        /// </summary>
        /// <returns></returns>
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
