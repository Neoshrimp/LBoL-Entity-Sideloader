using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Units;
using System;
using System.Collections.Generic;
using System.Text;
using static UnityEngine.UI.GridLayoutGroup;
using YamlDotNet.Core.Tokens;
using LBoL.EntityLib.Exhibits.Shining;
using LBoLEntitySideloader.Resource;
using UnityEngine;
using LBoL.Core.StatusEffects;
using LBoL.Presentation;
using LBoL.Base.Extensions;
using YamlDotNet.RepresentationModel;
using System.Linq;
using LBoL.Presentation.UI.Panels;
using LBoLEntitySideloader.Entities.DynamicTemplates;

namespace LBoLEntitySideloader.Entities
{
    public abstract class UltimateSkillTemplate : EntityDefinition,
        IConfigProvider<UltimateSkillConfig>,
        IGameEntityProvider<UltimateSkill>,
        IResourceConsumer<LocalizationOption>,
        IResourceConsumer<Sprite>
    {


        public override Type ConfigType() => typeof(UltimateSkillConfig);

        public override Type EntityType() => typeof(UltimateSkill);

        public override Type TemplateType() => typeof(UltimateSkillTemplate);

        /// <summary>
        /// Id: 
        /// Order: 
        /// PowerCost:  
        /// PowerPerLevel: PowerPerLevel x MaxPowerLevel = max power cap
        /// MaxPowerLevel: PowerPerLevel x MaxPowerLevel = max power cap
        /// RepeatableType: 
        /// Damage: 
        /// Value1: 
        /// Value2: 
        /// Keywords: Cosmetic Keywords. All Bombs have accuracy by default. Overwrite `Damage` getter to change that
        /// RelativeEffects: 
        /// RelativeCards: 
        /// </summary>
        /// <returns></returns>
        public UltimateSkillConfig DefaultConfig()
        {
            var config = new UltimateSkillConfig(
                Id: "",
                Order: 10,
                PowerCost: 100,
                PowerPerLevel: 100,
                MaxPowerLevel: 2,
                RepeatableType: UsRepeatableType.OncePerTurn,
                Damage: 1,
                Value1: 0,
                Value2: 0,
                Keywords: Keyword.Accuracy,
                RelativeEffects: new List<string>() { },
                RelativeCards: new List<string>() { }
                );

            return config;
        }

        public abstract UltimateSkillConfig MakeConfig();

        public abstract LocalizationOption LoadLocalization();
        /// <summary>
        /// Circle centered in 280x280 canvas but not touching the edges. Refer to ultSkillSpriteTemplate.png
        /// </summary>
        /// <returns></returns>
        public abstract Sprite LoadSprite();


        public void Consume(LocalizationOption locOption)
        {
            ProcessLocalization(locOption, EntityType());
        }

        public void Consume(Sprite sprite)
        {
            if (sprite == null)
                return;

            ResourcesHelper.Sprites[EntityType()].AlwaysAdd(UniqueId, sprite);
        }

        internal SpellTemplate CreateSpellTemplate()
        {
            var spell = new DynamicSpell
            {
                CreateLoadLoc = () => LoadLocalization(),
                CreateMakeConfig = () => new SpellConfig(UniqueId, "")
            };
            spell.Create(UniqueId, user);
            return spell;
        }

    }

}
