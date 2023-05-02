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

        public override Type ConfigType()
        {
            return typeof(StatusEffectConfig);
        }

        public override Type EntityType()
        {
            return typeof(StatusEffect);
        }

        public StatusEffectConfig DefaultConfig()
        {
            var statusEffectConfig = new StatusEffectConfig(
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
        /// IsVerbose: false,
        /// IsStackable: can the same status be added on top? currently all status effects.
        /// StackActionTriggerLevel: null,
        /// HasLevel: false,
        /// LevelStackType: StackType.Add,
        /// HasDuration: false,
        /// DurationStackType: StackType.Add,
        /// DurationDecreaseTiming: DurationDecreaseTiming.Custom,
        /// HasCount: false,
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
            ProcessLocalization(locOptions, (string key, Dictionary<string, object> value) => { TypeFactory<StatusEffect>._typeLocalizers.AlwaysAdd(key, value); });

        }
    }
}
