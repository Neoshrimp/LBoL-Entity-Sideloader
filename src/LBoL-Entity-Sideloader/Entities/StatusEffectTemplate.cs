using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.StatusEffects;
using LBoL.EntityLib.StatusEffects.Neutral.TwoColor;
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
                            ImageId: null,
                            VFX: "Default", 
                            VFXloop: "Default",
                            SFX: "Default"
                );
            return statusEffectConfig;
        }

        /// <summary>
        /// <para>
        /// <b>Identity and ordering</b><br/>
        /// <c>Id</c> — leave blank; it is assigned by <c>GetId()</c> at runtime.<br/>
        /// <c>Order</c> — default priority for reactors/handlers. 
        /// Priority argument on reactors/handlers can also be used instead. Priority 10 is normal<br/>
        /// <c>Type</c> — Positive, Negative, or Special. Special effects do not
        /// interact with most of the game's mechanics (e.g. Event Horizon's game-over effect).
        /// </para>
        /// <para>
        /// <b>Display</b><br/>
        /// <c>IsVerbose</c> — whether the effect has a "Brief" description. The brief
        /// text is set in YAML under the <c>Brief:</c> node and is used as the status
        /// description outside of battle. Set this when a generic effect (Firepower,
        /// Flawless, etc.) may need to appear as a card tooltip; otherwise false is fine.<br/>
        /// <c>Keywords</c> — keyworrds to show up as related tooltips; default <c>Keyword.None</c>.<br/>
        /// <c>RelativeEffects</c> — list of effect to show up as related tooltips (use <c>nameof(Seclass)</c>); default empty.
        /// <c>VFX</c> — presumably VFX when applying the SE? (Untested); default <c>"Default"</c>.<br/>
        /// <c>VFXloop</c> — vfx loop while status exists. See <see cref="Grace"/>'s configs; default <c>"Default"</c>.<br/>
        /// <c>SFX</c> — sound effect when applying the SE? (Untested); default <c>"Default"</c>.
        /// </para>
        /// <para>
        /// <b>Stacking and levels</b><br/>
        /// <c>IsStackable</c> — can the same status be applied on top of itself?
        /// Almost status effects set this true. Most unstackable just have no level.<br/>
        /// <c>HasLevel</c> — does the effect carry a stack count? Most ability effects
        /// do, letting them grow stronger when additional copies of the same ability
        /// are played. Fire of Ena (<see cref="ModuoluoFireSe"/>), for example, deals
        /// damage equal to <c>stacks × rainbow_mana_spent</c>.<br/>
        /// <c>LevelStackType</c> — how levels combine when the effect is applied while
        /// already present. Ignored when <c>HasLevel</c> is false:
        /// <br/>  • <c>StackType.Add</c> — sum the two levels (most common).
        /// <br/>  • <c>StackType.Max</c> — take the higher level.
        /// <br/>  • <c>StackType.Min</c> — take the lower level.
        /// <br/>  • <c>StackType.Keep</c> — keep the existing level.
        /// <br/>  • <c>StackType.Overwrite</c> — discard the existing level and use the new one.
        /// <br/>
        /// <c>StackActionTriggerLevel</c> — if set, the effect is removed and its
        /// <c>StatusAction</c> runs upon reaching this level. Charge
        /// (<see cref="Charging"/>) uses 8, applying Burst when it maxes out.
        /// </para>
        /// <para>
        /// <b>Duration</b><br/>
        /// <c>HasDuration</c> — does the status expire after a number of turns?
        /// Frail, Weak, Vulnerable, etc. all expire this way.<br/>
        /// <c>DurationStackType</c> — how durations combine; default <c>StackType.Add</c>.<br/>
        /// <c>DurationDecreaseTiming</c> — when the duration ticks down; default <c>DurationDecreaseTiming.Custom</c>.
        /// </para>
        /// <para>
        /// <b>Counters</b><br/>
        /// <c>HasCount</c> — whether the icon displays a counter for tracking an
        /// auxiliary value. Rain of Hell (<see cref="HekaHellRainSe"/>) uses it to show
        /// the total damage that will be dealt at end of turn.<br/>
        /// <c>CountStackType</c> — how counters combine; default <c>StackType.Keep</c>.<br/>
        /// <c>LimitStackType</c> — Undocumented; default <c>StackType.Keep</c>.<br/>
        /// <c>ShowPlusByLimit</c> — undocumented; default false.
        /// </para>
        /// </summary>
        public abstract StatusEffectConfig MakeConfig();

        /// <summary>
        /// 128x128 image
        /// </summary>
        /// <returns></returns>
        public abstract Sprite LoadSprite();

        public virtual ExtraIcons LoadExtraIcons() { return null; }

        public void Consume(Sprite sprite)
        {
            if (sprite != null)
                ResourcesHelper.Sprites[typeof(StatusEffect)].AlwaysAdd(UniqueId, sprite);

            var extraIcons = LoadExtraIcons();
            if (extraIcons != null)
                foreach (var kv in extraIcons.LoadMany())
                {
                    if (kv.Key != "" && kv.Value != null)
                        ResourcesHelper.Sprites[typeof(StatusEffect)].AlwaysAdd(UniqueId + kv.Key, kv.Value);
                }
        }

        public abstract LocalizationOption LoadLocalization();


        public void Consume(LocalizationOption locOptions)
        {
            ProcessLocalization(locOptions, EntityType());

        }
    }
}
