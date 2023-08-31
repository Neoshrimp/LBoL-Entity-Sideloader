using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.StatusEffects;
using LBoLEntitySideloader.Entities;
using LBoLEntitySideloader.Resource;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace LBoLEntitySideloader.TemplateGen
{
    public class ExhibitGen : TemplateGen<ExhibitTemplate>
    {

        public void QueueGen(IdContainer Id, bool overwriteVanilla, Func<ExhibitConfig> makeConfig = null, Func<ExhibitSprites> loadSprite = null, Func<LocalizationOption> loadLocalization = null, bool generateEmptyLogic = false)
        {

            var defClass = InnitDefintionType(Id, overwriteVanilla);

            MakeMethod(nameof(ExhibitTemplate.MakeConfig), makeConfig, defClass, overwriteVanilla);

            MakeMethod(nameof(ExhibitTemplate.LoadSprite), loadSprite, defClass, overwriteVanilla);

            MakeMethod(nameof(ExhibitTemplate.LoadLocalization), loadLocalization, defClass, overwriteVanilla);


            if (generateEmptyLogic)
            {
                MakeEntityLogic(Id, defClass, typeof(Exhibit));
            }
        }
    }
}
