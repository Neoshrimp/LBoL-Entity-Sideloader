using LBoL.ConfigData;
using LBoL.Core.Cards;
using LBoLEntitySideloader;
using LBoLEntitySideloader.Attributes;
using LBoLEntitySideloader.Entities;
using LBoLEntitySideloader.Resource;
using System;
using System.Collections.Generic;
using System.Text;

namespace FirstCard
{
    public sealed class FirstCardDef : CardTemplate
    {
        public override IdContainer GetId()
        {
            return nameof(FirstCard);   
        }
        
        public override CardImages LoadCardImages()
        {
            var imgs = new CardImages(BepinexPlugin.embeddedSource);

            imgs.AutoLoad(this, extension: ".png");

            return imgs;
        }

        public override LocalizationOption LoadLocalization()
        {
            throw new NotImplementedException();
        }

        public override CardConfig MakeConfig()
        {
            throw new NotImplementedException();
        }

        [EntityLogic(typeof(FirstCardDef))]
        public sealed class FirstCard : Card
        {
        }

    }
}
