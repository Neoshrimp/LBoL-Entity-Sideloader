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

        public override LocalizationOption LoadLocalization()
        {
            var globalLoc = new GlobalLocalization(embeddedSource);
            // for GlobalLocalization add each locale file only once per entity type (Card, StatusEffect etc.)
            globalLoc.LocalizationFiles.AddLocaleFile(Locale.En, "CardsEn");
            globalLoc.LocalizationFiles.AddLocaleFile(Locale.ZhHans, "CardsZhHans");


            return globalLoc;
        }


        [DontOverwrite]
        public override CardConfig MakeConfig()
        {
            return null;
        }


        // simply don't provide an EntityLogic type

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

        public override LocalizationOption LoadLocalization()
        {
            // !IMPORTANT. Since GlobalLocalization is being used and localization files are specified by SuikaBigballLoc already it's sufficient (and mandatory) to return simple GlobalLocalization object by only specifying a resource source in the constructor.
            var globalLoc = new GlobalLocalization(embeddedSource);
            return globalLoc;
        }


        [DontOverwrite]
        public override CardConfig MakeConfig()
        {
            return null;
        }
    }

    [OverwriteVanilla]
    // big problem with changing StatusEffect names: status effect names are hard localized in card description yaml..
    public sealed class FlawlessLoc : StatusEffectTemplate
    {
        public override IdContainer GetId()
        {
            return nameof(Invincible);
        }


        public override LocalizationOption LoadLocalization()
        {
            var globalLoc = new GlobalLocalization(embeddedSource, mergeTerms: true);
            // each type of entity needs to have their own global localization files
            globalLoc.LocalizationFiles.AddLocaleFile(Locale.En, "StatusEffectsEn");
            return globalLoc;
        }

        [DontOverwrite]
        public override Sprite LoadSprite()
        {
            // doesn't matter what these methods do. Since the have [DontOverwrite] they are never called.
            throw new NotImplementedException();
        }

        [DontOverwrite]
        public override StatusEffectConfig MakeConfig()
        {
            throw new NotImplementedException();
        }
    }


    // Exhibits are supported as well but no support for events (or anything else) yet.

}
