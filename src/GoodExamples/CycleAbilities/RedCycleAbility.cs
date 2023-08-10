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
using HarmonyLib;
using LBoL.Core.StatusEffects;
using UnityEngine.Rendering;
using LBoL.Core.Units;

namespace GoodExamples.CycleAbilities
{
    // to make Sideloader load a custom entity a public sealed class extending one of the templates must be created.
    // Currently implemented templates are CardTemplate, StatusEffectTemplate and ExhibitTemplate
    public sealed class RedCycleAbilityCardDefinition : CardTemplate
    {
        // Must return Id of the entity. All currently implemented templates use string as Id.
        // Id should be the same as the entity logic type name, hence, it's best to use nameof identifier
        public override IdContainer GetId()
        {
            return nameof(RedCycleAbilityCard);
        }

        // Create CardImages object. CardImages contain main and, optionally, sub and upgraded images for the card
        public override CardImages LoadCardImages()
        {

            var imgs = new CardImages(embeddedSource);
            // AutoLoad set and load images automatically if certain file naming criteria are met
            // main image must be named same as Id+<fileFormat>
            // [optional] sub images are name Id+<subIllustratorName>+fileFormat>
            // [optional] upgraded image Id+"Upgrade+<fileFormat>"
            // all images must have the same file extension and be in the same folder

            // relative path = path to image folder. In embeddedSource folders are separated by '.'
            imgs.AutoLoad(this, ".png", relativePath: "CycleAbilities.");

            // alternatively, CardImages.main, CardImages.subs etc. can be initialized manually using ResourceLoader.LoadTexture()
            return imgs;
        }

        public override LocalizationOption LoadLocalization()
        {
            // use global localization file. Global file for cards was already specified in FistOfTheThreeFairies.cs
            // so it's sufficient to return empty GlobalLocalization() to indicate that global localizations is used
            return new GlobalLocalization(embeddedSource);
        }

        // defines basic information for the card. Should never be called manually. 
        // if some information is needed from this config it can be accessed by calling
        // CardConfig.FromId(new RedCycleAbilityCardDefinition().UniqueId) 
        public override CardConfig MakeConfig()
        {
            CardConfig config = new CardConfig(
                // get the next number from CardConfig index sequence
                Index: sequenceTable.Next(typeof(CardConfig)),
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
                Rarity: Rarity.Uncommon,
                Type: CardType.Ability,
                TargetType: TargetType.Self,
                Colors: new List<ManaColor>() { ManaColor.Colorless, ManaColor.Red },
                IsXCost: false,
                Cost: new ManaGroup() { Colorless = 1, Red = 1, Any = 1 },
                UpgradedCost: new ManaGroup() { Any = 1, Red = 1 },
                MoneyCost: null,
                Damage: null,
                UpgradedDamage: null,
                Block: null,
                UpgradedBlock: null,
                Shield: null,
                UpgradedShield: null,
                Value1: 6,
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
                PassiveCost: null,
                UpgradedPassiveCost: null,
                ActiveCost: null,
                UpgradedActiveCost: null,
                UltimateCost: null,
                UpgradedUltimateCost: null,

                Keywords: Keyword.None,
                UpgradedKeywords: Keyword.None,
                EmptyDescription: false,
                RelativeKeyword: Keyword.Basic,
                UpgradedRelativeKeyword: Keyword.Basic,

                RelativeEffects: new List<string>() { },
                UpgradedRelativeEffects: new List<string>() { },
                RelativeCards: new List<string>() { },
                UpgradedRelativeCards: new List<string>() { },
                Owner: null,
                Unfinished: false,
                Illustrator: "???",
                SubIllustrator: new List<string>() { }
             );

            return config;
        }

        // card behavior. Ability card behavior are usually is very simple, they add ability StatusEffect.
        // The actual behavior of the ability is implemented as StatusEffect.
        // Entity logic type must be specified by adding EntityLogic attribute on public sealed class
        // extending the correct base entity type. In this case it's a card.
        [EntityLogic(typeof(RedCycleAbilityCardDefinition))]
        public sealed class RedCycleAbilityCard : Card
        {
            protected override IEnumerable<BattleAction> Actions(UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
            {
                // it's important (particularly when overwriting vanilla entities) to make sure that
                // BuffAction refers to your own implementation of StatusEffect.
                // Full type name can be specified to prevent human error.
                yield return BuffAction<GoodExamples.CycleAbilities.RedCycleAbilitySeDefinition.RedCycleAbilitySe>(Value1);

            }
        }

    }




    public sealed class RedCycleAbilitySeDefinition : StatusEffectTemplate
    {

        // Must return Id of the entity. All currently implemented templates use string as Id.
        // Id should be the same as the entity logic type name, hence, it's best to use nameof identifier
        public override IdContainer GetId()
        {
            return nameof(RedCycleAbilitySe);
        }

        public override LocalizationOption LoadLocalization()
        {
            // LocalizationOption allows to return LocalizationFiles directly, however,
            // then only single entity localization can be contained within a the file.
            var locFiles = new LocalizationFiles(embeddedSource);
            // folders in embedded source are separated by '.'
            locFiles.AddLocaleFile(Locale.En, "CycleAbilities.RedCycleAbilitySeEn.yaml");

            return locFiles;
        }

        public override Sprite LoadSprite()
        {
            // since status effect only need single icon sprite should be loaded using resource loader directly
            return ResourceLoader.LoadSprite("RedCycleAbilitySe.png", embeddedSource);
        }


        // defines basic information for the card. Should never be called manually. 
        // if some information is needed from this config it can be accessed by calling
        // StatusEffectConfig.FromId(new RedCycleAbilitySeDefinition().UniqueId) 
        public override StatusEffectConfig MakeConfig()
        {
            var statusEffectConfig = new StatusEffectConfig(
                Id: "",
                Order: 10,
                Type: StatusEffectType.Positive,
                IsVerbose: false,
                IsStackable: true,
                StackActionTriggerLevel: null,
                HasLevel: true,
                LevelStackType: StackType.Add,
                HasDuration: false,
                DurationStackType: StackType.Add,
                DurationDecreaseTiming: DurationDecreaseTiming.Custom,
                HasCount: false,
                CountStackType: StackType.Keep,
                LimitStackType: StackType.Keep,
                ShowPlusByLimit: false,
                Keywords: Keyword.Basic,
                RelativeEffects: new List<string>() { },
                VFX: "Default",
                VFXloop: "Default",
                SFX: "Default"
            );
            return statusEffectConfig;
        }



        [EntityLogic(typeof(RedCycleAbilitySeDefinition))]
        public sealed class RedCycleAbilitySe : StatusEffect
        {
            protected override void OnAdded(Unit unit)
            {
                ReactOwnerEvent(Battle.CardUsed, new EventSequencedReactor<CardUsingEventArgs>(OnCardPlayed));
            }

            private IEnumerable<BattleAction> OnCardPlayed(CardUsingEventArgs args)
            {
                if (args.Card.Keywords.HasFlag(Keyword.Basic))
                {
                    // makes status icon flash
                    NotifyActivating();
                    // reaction dmg = attack damage which doesn't scale with firepower, status modifiers etc., fire of Ena gun vfx
                    yield return new DamageAction(Battle.Player, Battle.EnemyGroup.Alives, DamageInfo.Reaction(Level), "秽火", GunType.Single);
                }
            }
        }
    }


}
