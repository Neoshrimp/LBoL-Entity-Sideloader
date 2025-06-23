using LBoL.ConfigData;
using LBoL.Core.StatusEffects;
using LBoL.Core.Units;
using LBoL.EntityLib.StatusEffects.Cirno;
using LBoL.Presentation;
using LBoLEntitySideloader;
using LBoLEntitySideloader.Attributes;
using LBoLEntitySideloader.Entities;
using LBoLEntitySideloader.Resource;
using System;
using UnityEngine;



namespace Random_Examples
{
    [OverwriteVanilla]
    public sealed class StatusEffectExamples : StatusEffectTemplate
    {
        public override IdContainer GetId() => nameof(Cold);

        [DontOverwrite]
        public override LocalizationOption LoadLocalization()
        {
            throw new NotImplementedException();
        }

        [DontOverwrite]
        public override Sprite LoadSprite()
        {
            throw new NotImplementedException();
        }



        public override StatusEffectConfig MakeConfig()
        {
            var config = StatusEffectConfig.FromId(GetId());
            config.IsStackable = false;
            return config;
        }
    }


    public sealed class IconChangeStatusDef : StatusEffectTemplate
    {
        public override IdContainer GetId() => nameof(IconChangeStatus);

        public override LocalizationOption LoadLocalization()
        {
            return null;
        }

        public override Sprite LoadSprite()
        {
            return null;
        }

        public const string icon1Key = "1";
        public const string icon2Key = "2";


        public override ExtraIcons LoadExtraIcons()
        {
            var ei = new ExtraIcons();
            ei.icons.Add(icon1Key, ResourceLoader.LoadSprite("bea.png", BepinexPlugin.directorySource));
            ei.icons.Add(icon2Key, ResourcesHelper.Sprites[typeof(StatusEffect)][nameof(WindGirl)]);

            return ei;
        }

        public override StatusEffectConfig MakeConfig()
        {
            return DefaultConfig();
        }
    }

    [EntityLogic(typeof(IconChangeStatusDef))]
    public sealed class IconChangeStatus : StatusEffect
    {
        public override string OverrideIconName 
        { 
            get
            {
                if(Battle?.RoundCounter == 1)
                    return Id + IconChangeStatusDef.icon1Key;
                if (Battle?.RoundCounter >= 2)
                    return Id + "2";
                return Id;
            } 
        }

        protected override void OnAdded(Unit unit)
        {
            // refreshed icon at the start of the round
            HandleOwnerEvent(Battle.RoundStarted, args => NotifyChanged());
        }
    }
}
