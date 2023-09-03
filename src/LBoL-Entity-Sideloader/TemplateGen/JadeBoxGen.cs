using LBoL.ConfigData;
using LBoL.Core;
using LBoLEntitySideloader.Entities;
using LBoLEntitySideloader.Resource;
using System;
using System.Collections.Generic;
using System.Text;

namespace LBoLEntitySideloader.TemplateGen
{
    public class JadeBoxGen : TemplateGen<JadeBoxTemplate>
    {

        public void QueueGen(IdContainer Id, bool overwriteVanilla, Func<JadeBoxConfig> makeConfig = null, Func<LocalizationOption> loadLocalization = null, bool generateEmptyLogic = false)
        {

            var defClass = InnitDefintionType(Id, overwriteVanilla);

            MakeMethod(nameof(JadeBoxTemplate.MakeConfig), makeConfig, defClass, overwriteVanilla);

            MakeMethod(nameof(JadeBoxTemplate.LoadLocalization), loadLocalization, defClass, overwriteVanilla);


            if (generateEmptyLogic)
            {
                MakeEntityLogic(Id, defClass, typeof(JadeBox));
            }
        }
    }
}
