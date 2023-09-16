using Cysharp.Threading.Tasks;
using LBoL.ConfigData;
using LBoL.EntityLib.PlayerUnits;
using LBoL.Presentation;
using LBoLEntitySideloader;
using LBoLEntitySideloader.Attributes;
using LBoLEntitySideloader.Entities;
using LBoLEntitySideloader.Resource;
using LBoLEntitySideloader.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Random_Examples
{
    [OverwriteVanilla]
    public sealed class ReimuIsSunnyDef : UnitModelTemplate
    {
        public override IdContainer GetId() => nameof(Reimu);

        
        public override ModelOption LoadModelOptions()
        {
            return new ModelOption(ResourcesHelper.LoadSpineUnitAsync("Sunny"));
        }

        
        public override UniTask<Sprite> LoadSpellSprite()
        {
            return ResourcesHelper.LoadSpellPortraitAsync("Sunny");
        }

        public override UnitModelConfig MakeConfig()
        {
            // deep copy config so original in unaffected
            var config = UnitModelConfig.FromName("Sunny").Copy();
            config.Flip = true;
            return config;
        }
    }
}
