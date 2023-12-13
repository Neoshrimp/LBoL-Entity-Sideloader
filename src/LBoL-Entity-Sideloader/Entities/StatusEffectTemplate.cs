using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.StatusEffects;
using LBoL.Presentation;
using LBoLEntitySideloader.Resource;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace LBoLEntitySideloader.Entities
{
    public abstract class StatusEffectTemplate : EntityDefinition,
        IConfigProvider<StatusEffectConfig>,
        IGameEntityProvider<StatusEffect>,
        IResourceConsumer<LocalizationOption>,
        IResourceConsumer<Sprite>
    {

        public override Type TemplateType()
        {
            return typeof(StatusEffectTemplate);
        }

        public override Type ConfigType()
        {
            return typeof(StatusEffectConfig);
        }

        public override Type EntityType()
        {
            return typeof(StatusEffect);
        }

        /// <summary>
        /// Common default values for StatusEffectConfig. The values which are safe to be left as a null are left as null.
        /// </summary>
        /// <returns></returns>
        public StatusEffectConfig DefaultConfig()
        {
            var statusEffectConfig = new StatusEffectConfig(
                            Index: 0,
                            Id: "",
                            Order: 10,
                            Type: StatusEffectType.Positive,
                            IsVerbose: false,
                            IsStackable: true,
                            StackActionTriggerLevel: null,
                            HasLevel: false,
                            LevelStackType: StackType.Add,
                            HasDuration: false,
                            DurationStackType: StackType.Add,
                            DurationDecreaseTiming: DurationDecreaseTiming.Custom,
                            HasCount: false,
                            CountStackType: StackType.Keep,
                            LimitStackType: StackType.Keep,
                            ShowPlusByLimit: false,
                            Keywords: Keyword.None,
                            RelativeEffects: new List<string>() { },
                            VFX: "Default", 
                            VFXloop: "Default",
                            SFX: "Default"
                );
            return statusEffectConfig;
        }

        /// <summary>
        /// Id: should be left blank since it's eventually set by GetId() anyway.
        /// Order: default priority for reactors/handlers. priority argument on reactors/handlers can be used instead. Priority 10 is normal
        /// Type: Positive, negative or special. Special is something which doesn't interact with most of the game's mechanics i.e. Event Horizon's game over effect.
        /// IsVerbose: does the effect have 'Brief' description? Brief description can be specified in yaml as 'Brief:' node. Brief will be used as status effect description outside of battle. Important if the status effect is generic, like Firepower or Flawless, and might need to be displayed as tooltip for cards. But for most cases it's fine to leave this false.
        /// IsStackable: can the same status be added on top? Currently all status effects set to true but some of them doesn't have a level.
        /// StackActionTriggerLevel: should effect status effect be Removed and execute StatusAction when reaching a certain level? Charge (Id:Charging) has StackActionTriggerLevel set to 8. When Charge reaches 8 it applies Burst through its StackAction.
        /// HasLevel: does the StatusEffect have stack count? Most of ability status effects have stacks which allows them to get stronger when additional copies of the same ability are played. For example, Fire of Ena (Id:ModuoluoFireSe) deal damage equal to amount_of_stacks x rainbow_mana_spent.
        /// LevelStackType: how should effects levels stack when an effect is applied while the same effect is already present? This property is irrelevant if HasLevel is false.
        /// StackType.Add: add the two levels. The most common option.
        /// StackType.Max: use the higher level.
        /// StackType.Min: use the lower level.
        /// StackType.Keep: always keep already present level.
        /// StackType.Max: discard the present level and use the new one.
        /// HasDuration: does the status disappears after certain number of turns? Frail, Weak, Vuln ect. all expire after certain amount of turns.
        /// DurationStackType: StackType.Add,
        /// DurationDecreaseTiming: DurationDecreaseTiming.Custom,
        /// HasCount: has a counter? Number displayed on status effect icon for keeping track of some additional effects. For example, Rain of Hell (Id:HekaHellRainSe) use it to show total amount of damage which would be dealt at the end of the turn.
        /// CountStackType: StackType.Keep,
        /// LimitStackType: StackType.Keep,
        /// ShowPlusByLimit: false,
        /// Keywords: Keyword.None,
        /// RelativeEffects: new List<string>() { },
        /// VFX: "Default",
        /// VFXloop: "Default",
        /// SFX: "Default"
        /// </summary>
        /// <returns></returns>
        public abstract StatusEffectConfig MakeConfig();

        /// <summary>
        /// 128x128 image
        /// </summary>
        /// <returns></returns>
        public abstract Sprite LoadSprite();

        public void Consume(Sprite sprite)
        {
            if (sprite == null)
                return;

            ResourcesHelper.Sprites[typeof(StatusEffect)].AlwaysAdd(UniqueId, sprite);
        }

        public abstract LocalizationOption LoadLocalization();


        public void Consume(LocalizationOption locOptions)
        {
            ProcessLocalization(locOptions, EntityType());

        }
    }
}
