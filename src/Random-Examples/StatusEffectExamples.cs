using LBoL.ConfigData;
using LBoL.EntityLib.StatusEffects.Cirno;
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
}
