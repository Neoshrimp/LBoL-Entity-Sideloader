using Cysharp.Threading.Tasks;
using LBoL.ConfigData;
using LBoL.EntityLib.PlayerUnits;
using LBoL.Presentation;
using LBoLEntitySideloader;
using LBoLEntitySideloader.Attributes;
using LBoLEntitySideloader.Entities;
using LBoLEntitySideloader.Resource;
using LBoLEntitySideloader.Resource.AsyncTextureImport;
using LBoLEntitySideloader.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static Random_Examples.BepinexPlugin;


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
    [OverwriteVanilla]
    public sealed class MarisaIsYoumuDef : UnitModelTemplate
    {
        public override IdContainer GetId() => "Marisa";
        

        public override ModelOption LoadModelOptions()
        {
            // pixels per unit should be pretty high
            return new ModelOption(ResourceLoader.LoadSpriteAsync("Youmu.png", directorySource, ppu: 565, anisoLevel: 8, filterMode:FilterMode.Trilinear));

        }


        public override UniTask<Sprite> LoadSpellSprite()
        {
            return ResourceLoader.LoadSpriteAsync("ghibli.jpg", directorySource);
        }

        public override UnitModelConfig MakeConfig()
        {
            var config = UnitModelConfig.FromName("Youmu").Copy();
            config.Flip = false;
            config.Type = 0;
            config.Offset = new Vector2(0, 0.04f);
            return config;
        }
    }


}
