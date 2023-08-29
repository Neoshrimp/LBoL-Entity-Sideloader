using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom.Compiler;
using System.CodeDom;
using LBoLEntitySideloader.Resource;
using LBoL.ConfigData;
using LBoLEntitySideloader.Entities;

namespace LBoLEntitySideloader.TemplateGen
{
    public class CardGen : TemplateGen<CardTemplate>
    {



        public void QueueGen(IdContainer Id, bool overwriteVanilla = false, Func < CardConfig> makeConfig = null, Func<CardImages> loadCardImages = null, Func<LocalizationOption> loadLocalization = null)
        {
            var defClass = InnitDefintionType(Id, overwriteVanilla);

            //2do overwrite logic

            MakeMethod(nameof(CardTemplate.MakeConfig), makeConfig, defClass, overwriteVanilla);

            MakeMethod(nameof(CardTemplate.LoadCardImages), loadCardImages, defClass, overwriteVanilla);

            MakeMethod(nameof(CardTemplate.LoadLocalization), loadLocalization, defClass, overwriteVanilla);




            //2do entity type
        }

    }
}
    