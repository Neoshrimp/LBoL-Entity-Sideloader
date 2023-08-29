using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Cards;
using LBoL.Core.StatusEffects;
using LBoL.Core.Units;
using LBoL.EntityLib.Cards.Character.Marisa;
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

    // NOT sealed, this attribute is inherited
    [OverwriteVanilla]
    public abstract class MassLocBase : CardTemplate
    {

        [DontOverwrite]
        public override CardImages LoadCardImages()
        {
            throw new NotImplementedException();
        }

        public override LocalizationOption LoadLocalization()
        {
            return new GlobalLocalization(embeddedSource, mergeTerms: true);
        }

        [DontOverwrite]
        public override CardConfig MakeConfig()
        {
            throw new NotImplementedException();
        }
    }

    public sealed class RedGiantStarLoc : MassLocBase
    {
        public override IdContainer GetId() => nameof(RedGiantStar);

        // set global localization files once
        // 2do if this method fails due to loading dupes entire globalLocalization wont load
        public override LocalizationOption LoadLocalization()
        {
            var globalLoc = new GlobalLocalization(embeddedSource, mergeTerms: true);
            globalLoc.LocalizationFiles.AddLocaleFile(Locale.En, "CardsEn");
            return globalLoc;
        }
    }

    // spam this shit

    public sealed class MassLoc0 : MassLocBase { public override IdContainer GetId() => nameof(HuoliQuankai); }
    public sealed class MassLoc1 : MassLocBase { public override IdContainer GetId() => nameof(MasterOfCollection); }
    public sealed class MassLoc2 : MassLocBase { public override IdContainer GetId() => nameof(ChargingPotion); }
    public sealed class MassLoc3 : MassLocBase { public override IdContainer GetId() => nameof(HuoliQuankai); }
    public sealed class MassLoc4 : MassLocBase { public override IdContainer GetId() => nameof(StarlightTaifengSummoner); }
    public sealed class StarlightTaifengLoc : MassLocBase { public override IdContainer GetId() => nameof(StarlightTaifeng); }
    public sealed class FindCollectionLoc : MassLocBase { public override IdContainer GetId() => nameof(FindCollection); }
    public sealed class WhiteLaserLoc : MassLocBase { public override IdContainer GetId() => nameof(WhiteLaser); }
    public sealed class BlazingStarLoc : MassLocBase { public override IdContainer GetId() => nameof(BlazingStar); }
    public sealed class SplitPotionLoc : MassLocBase { public override IdContainer GetId() => nameof(SplitPotion); }
    public sealed class IceSparkLoc : MassLocBase { public override IdContainer GetId() => nameof(IceSpark); }




}
