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

        /*        public override Type Generate(Func<CardConfig> makeConfigFunc, bool overwriteVanilla = false)
                {
                    return base.Generate(makeConfigFunc, overwriteVanilla);


                }*/

        /*        public Type Generate(Func<> makeConfigFunc, bool overwriteVanilla = false, Func<CardImages> loadCardImiagesFunc)
                {

                }*/


        public void QueueGen(IdContainer Id, Func<CardConfig> makeConfig, Func<CardImages> loadCardImages, Func<LocalizationOption> loadLocalization, bool overwriteVanilla = false)
        {
            var defClass = InnitDefintionType(Id, overwriteVanilla);

            //2do overwrite logic

            MakeMethod(nameof(CardTemplate.MakeConfig), makeConfig, defClass);


            MakeMethod(nameof(CardTemplate.LoadCardImages), loadCardImages, defClass);

            MakeMethod(nameof(CardTemplate.LoadLocalization), loadLocalization, defClass);



        }

    }
}
    