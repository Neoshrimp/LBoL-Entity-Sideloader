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

    public sealed class RedCycleAbilityCardDefinition : CardTemplate
    {
        public override IdContainer GetId()
        {
            return nameof(RedCycleAbilityCard);
        }

        public override CardImages LoadCardImages()
        {
            var imgs = new CardImages(embeddedSource);
            imgs.AutoLoad(this, ".png", relativePath: "CycleAbilities.");
            return imgs;
        }

        public override LocalizationOption LoadLocalization()
        {
            return new GlobalLocalization();
        }

        public override CardConfig MakeConfig()
        {
            CardConfig config = new CardConfig(
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


        [EntityLogic(typeof(RedCycleAbilityCardDefinition))]
        public sealed class RedCycleAbilityCard : Card
        {
            protected override IEnumerable<BattleAction> Actions(UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
            {
                yield return BuffAction<RedCycleAbilitySe>(Value1);

            }
        }

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
                NotifyActivating();
                // reaction dmg = attack damage which doesn't scale with firepower, status modifiers etc., fire of Ena gun vfx
                yield return new DamageAction(Battle.Player, Battle.EnemyGroup.Alives, DamageInfo.Reaction(Level), "秽火", GunType.Single);
            }
        }
    }


    public sealed class RedCycleAbilitySeDefinition : StatusEffectTemplate
    {
        public override IdContainer GetId()
        {
            return nameof(RedCycleAbilitySe);
        }

        public override LocalizationOption LoadLocalization()
        {
            // LocalizationOption allows to return LocalizationFiles directly, however, then only one entity localization can be contained in a the file.
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
    }
}
