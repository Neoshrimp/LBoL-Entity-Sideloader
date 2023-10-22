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

namespace LBoLEntitySideloader.Entities
{
    public abstract class UltimateSkillTemplate : EntityDefinition,
        IConfigProvider<UltimateSkillConfig>,
        IGameEntityProvider<UltimateSkill>,
        IResourceConsumer<LocalizationOption>,
        IResourceConsumer<Sprite>
    {

        public const string OnCastTitle = "OnCastTitle";
        public const string OnCastName = "OnCastName";
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

            if (locOption is GlobalLocalization globalLoc)
            {
                if (globalLoc.LocalizationFiles.locTable.NotEmpty())
                {
                    if (!UniqueTracker.Instance.spellEntriesLocFiles.TryAdd(userAssembly, globalLoc.LocalizationFiles))
                    {
                        Log.LogDev()?.LogWarning($"{userAssembly.GetName().Name}: {GetType()} tries to set global spell localization files but they've already been set by another {TemplateType().Name}.");
                    }
                }
                UniqueTracker.Instance.spellIdsToLocalize.TryAdd(userAssembly, new HashSet<string>());
                UniqueTracker.Instance.spellIdsToLocalize[userAssembly].Add(GetId());
                return;
            }


        }

        public void Consume(Sprite sprite)
        {
            if (sprite == null)
                return;

            ResourcesHelper.Sprites[EntityType()].AlwaysAdd(UniqueId, sprite);
        }

        // 2do move to SpellConfig?
        internal static void LoadAllSpecialLoc(SpellPanel spellPanel = null)
        {
            foreach (var usT in UniqueTracker.Instance.ultimateSkillTemplates)
            {
                usT.LoadSpecialLoc(usT.LoadLocalization(), spellPanel);
            }
        }

        /// <summary>
        /// on cast spell title
        /// </summary>
        /// <param name="locOption"></param>
        internal void LoadSpecialLoc(LocalizationOption locOption, SpellPanel spellPanel)
        {

            if (locOption is GlobalLocalization)
            {
                if (UniqueTracker.Instance.spellEntriesLocFiles.TryGetValue(user.assembly, out var spellLocfiles))
                {

                    LocalizationOption.FillSpellPanelLocTable(spellLocfiles.LoadLocTable(UniqueTracker.Instance.spellIdsToLocalize[user.assembly].ToArray()), spellLocfiles.mergeTerms, spellPanel);
                }
                return;
            }

            if (locOption is LocalizationFiles locFiles)
            {

                var termDic = locFiles.LoadLocTable(new string[] { GetId() });
                LocalizationOption.FillSpellPanelLocTable(termDic, locFiles.mergeTerms, spellPanel);

                return;
            }

            if (locOption is DirectLocalization rawLoc)
            {

                var termDic = rawLoc.WrapTermDic(UniqueId);

                LocalizationOption.FillSpellPanelLocTable(termDic, rawLoc.mergeTerms, spellPanel);

                return;
            }
        }
    }

}
