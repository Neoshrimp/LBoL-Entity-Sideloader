using LBoL.ConfigData;
using LBoL.EntityLib.Cards.Character.Sakuya;
using LBoL.Presentation;
using LBoLEntitySideloader;
using LBoLEntitySideloader.Attributes;
using LBoLEntitySideloader.Entities;
using LBoLEntitySideloader.Resource;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Random_Examples
{
    [OverwriteVanilla]
    public class TimeWalkCardDef : CardTemplate
    {
        public override IdContainer GetId() => nameof(TimeWalk);


        public override CardImages LoadCardImages()
        {
            var imgs = new CardImages(BepinexPlugin.embeddedSource);
            imgs.main = (Texture2D)ResourcesHelper.TryGetCardImage(nameof(TimeWalk));
            return imgs;

        }

        [DontOverwrite]
        public override LocalizationOption LoadLocalization() => null;

        [DontOverwrite]
        public override CardConfig MakeConfig() => null;

    }
}
