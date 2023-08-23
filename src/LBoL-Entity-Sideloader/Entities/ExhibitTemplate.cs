using LBoL.Base;
using LBoL.Base.Extensions;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Cards;
using LBoL.Core.StatusEffects;
using LBoL.Presentation;
using LBoLEntitySideloader.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;
using static LBoLEntitySideloader.BepinexPlugin;

namespace LBoLEntitySideloader.Entities
{

    // 2do TEST ExhibitInfoAttribute for custom exhibits (same for adventures)
    public abstract class ExhibitTemplate : EntityDefinition,
        IConfigProvider<ExhibitConfig>,
        IGameEntityProvider<Exhibit>,
        IResourceConsumer<LocalizationOption>,
        IResourceConsumer<ExhibitSprites>
    {

        public override Type TemplateType()
        {
            return typeof(ExhibitTemplate);
        }
        public override Type ConfigType()
        {
            return typeof(ExhibitConfig);
        }

        public override Type EntityType()
        {
            return typeof(Exhibit);
        }


        public ExhibitConfig DefaultConfig()
        {
            var exhibitConfig = new ExhibitConfig(
                Index: 0,
                Id: "",
                Order: 10,
                IsDebug: false,
                IsPooled: true,
                IsSentinel: false,
                Revealable: false,
                Appearance: AppearanceType.NonShop,
                Owner: "",
                LosableType: ExhibitLosableType.Losable,
                Rarity: Rarity.Common,
                Value1: null,
                Value2: null,
                Value3: null,
                Mana: null,
                BaseManaRequirement: null,
                BaseManaColor: null,
                BaseManaAmount: 0,
                HasCounter: false,
                InitialCounter: null,
                Keywords: Keyword.None,
                RelativeEffects: new List<string>() { },
                RelativeCards: new List<string>() { }
                );

            return exhibitConfig;
        }

        public abstract ExhibitConfig MakeConfig();


        public abstract ExhibitSprites LoadSprite();

        public void Consume(ExhibitSprites sprites)
        {
            if (sprites == null)
                return;

            if (sprites.main != null)
                ResourcesHelper.Sprites[typeof(Exhibit)].AlwaysAdd(UniqueId, sprites.main);
            else
                Log.LogDev()?.LogWarning($"Exhibit {UniqueId} of {this.GetType().Name} definition doesn't have the main sprite set.");

            foreach (var kv in sprites.LoadMany())
            {
                if(kv.Value != null)
                    ResourcesHelper.Sprites[typeof(Exhibit)].AlwaysAdd(UniqueId + kv.Key, kv.Value);
            }


        }

        public abstract LocalizationOption LoadLocalization();


        public void Consume(LocalizationOption locOptions)
        {


            ProcessLocalization(locOptions, EntityType());
        }




    }
}
