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
    public class MultiColorManaDef
    {
        [OverwriteVanilla]
        public sealed class SuikaBigballLoc : CardTemplate
        {
            public override IdContainer GetId()
            {
                return nameof(SuikaBigball);
            }

            [DontOverwrite]
            public override CardImages LoadCardImages()
            {
                return null;
            }


            [DontOverwrite]

            public override LocalizationOption LoadLocalization()
            {
                return null;
            }


            public override CardConfig MakeConfig()
            {
                var config = CardConfig.FromId(nameof(SuikaBigball));
                config.Cost = new ManaGroup() { Any = 1, Black = 1, Blue = 1, Colorless = 1, Green = 1, Philosophy = 1, Red = 1, White = 1 };
                return config;
            }



        }


        [OverwriteVanilla]
        public sealed class YinglangHowlLoc : CardTemplate
        {
            public override IdContainer GetId()
            {
                return nameof(YinglangHowl);
            }

            [DontOverwrite]
            public override CardImages LoadCardImages()
            {
                return null;
            }

            [DontOverwrite]
            public override LocalizationOption LoadLocalization()
            {
                return null;
            }


            public override CardConfig MakeConfig()
            {
                var config = CardConfig.FromId(nameof(YinglangHowl));
                config.Colors = new List<ManaColor>() { ManaColor.Black, ManaColor.Blue, ManaColor.White, ManaColor.Red };
                config.Cost = new ManaGroup() { Black = 4, Blue = 4};
                return config;
            }
        }
    }
}
