using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Cards;
using LBoL.Core.StatusEffects;
using LBoL.Core.Units;
using LBoL.EntityLib.Cards.Neutral.Red;
using LBoLEntitySideloader;
using LBoLEntitySideloader.Attributes;
using LBoLEntitySideloader.Entities;
using LBoLEntitySideloader.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Random_Examples.BepinexPlugin;

namespace Random_Examples
{
    [OverwriteVanilla]
    public sealed class SuikaBigballDefinition : CardTemplate
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

        public override LocalizationOption LoadLocalization()
        {
            var globalLoc = new GlobalLocalization(embeddedSource);
            globalLoc.LocalizationFiles.AddLocaleFile(Locale.En, "CardsEn");

            return globalLoc;
        }

        [DontOverwrite]
        public override CardConfig MakeConfig()
        {
            return null;
        }


    }



}
