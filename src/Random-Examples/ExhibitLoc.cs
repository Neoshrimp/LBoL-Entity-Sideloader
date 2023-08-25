using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Cards;
using LBoL.Core.StatusEffects;
using LBoL.Core.Units;
using LBoL.EntityLib.Cards.Neutral.Red;
using LBoL.EntityLib.Exhibits.Shining;
using LBoL.Presentation.UI.Panels;
using LBoLEntitySideloader;
using LBoLEntitySideloader.Attributes;
using LBoLEntitySideloader.Entities;
using LBoLEntitySideloader.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using static Random_Examples.BepinexPlugin;

namespace Random_Examples
{
    [OverwriteVanilla]
    public sealed class LunarVeilDef : ExhibitTemplate
    {
        public override IdContainer GetId()
        {
            return "YueYuyi";
        }

        public override LocalizationOption LoadLocalization()
        {
            var gl = new GlobalLocalization(embeddedSource, mergeTerms: true);
            //var gl = new GlobalLocalization(embeddedSource);
            gl.LocalizationFiles.AddLocaleFile(Locale.En, "ExhibitsEn");
            return gl;
        }


        [DontOverwrite]
        public override ExhibitSprites LoadSprite()
        {
            throw new NotImplementedException();
        }

        [DontOverwrite]
        public override ExhibitConfig MakeConfig()
        {
            var c = ExhibitConfig.FromId(GetId());
            c.Rarity = Rarity.Shining;
            return c;
        }
    }
}
