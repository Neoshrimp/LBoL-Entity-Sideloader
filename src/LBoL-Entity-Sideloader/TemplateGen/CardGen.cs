using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom.Compiler;
using System.CodeDom;
using LBoLEntitySideloader.Resource;
using LBoL.ConfigData;
using LBoLEntitySideloader.Entities;
using LBoL.Core.Cards;

namespace LBoLEntitySideloader.TemplateGen
{
    public class CardGen : TemplateGen<CardTemplate>
    {



        public void QueueGen(IdContainer Id, bool overwriteVanilla, Func < CardConfig> makeConfig = null, Func<CardImages> loadCardImages = null, Func<LocalizationOption> loadLocalization = null, bool generateEmptyLogic = false)
        {
            var defClass = InnitDefintionType(Id, overwriteVanilla);

            MakeMethod(nameof(CardTemplate.MakeConfig), makeConfig, defClass, overwriteVanilla);

            MakeMethod(nameof(CardTemplate.LoadCardImages), loadCardImages, defClass, overwriteVanilla);

            MakeMethod(nameof(CardTemplate.LoadLocalization), loadLocalization, defClass, overwriteVanilla);


            if (generateEmptyLogic)
            {
                MakeEntityLogic(Id, defClass, typeof(Card));
            }


        }

    }
}
    