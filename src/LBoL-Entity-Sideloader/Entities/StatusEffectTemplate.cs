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
                            /// id. fine to leave blank since it's eventually set by GetId() anyway
                            Id: "",
                            /// default priority for reactors/handlers. priority argument on reactors/handlers can be used instead
                            Order: 10,
                            /// positive, negative or special. Special is something which doesn't interact with most of the game's mechanics i.e. Event Horizon's game over effect.
                            Type: StatusEffectType.Positive,
                            IsVerbose: false,
                            /// can the same status be added on top? currently all status effects 
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

        public abstract LocalizationOption LoadText();


        public void Consume(LocalizationOption locOptions)
        {
            ProcessLocalization(locOptions, (string key, Dictionary<string, object> value) => { TypeFactory<StatusEffect>._typeLocalizers.AlwaysAdd(key, value); });

        }
    }
}
