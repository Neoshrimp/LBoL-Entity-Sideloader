using LBoL.ConfigData;
using LBoL.Core.Cards;
using LBoL.Core.StatusEffects;
using LBoLEntitySideloader.Entities;
using LBoLEntitySideloader.Resource;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace LBoLEntitySideloader.TemplateGen
{
    public class StatusEffectGen : TemplateGen<StatusEffectTemplate>
    {

        public void QueueGen(IdContainer Id, bool overwriteVanilla, Func<StatusEffectConfig> makeConfig = null, Func<Sprite> loadSprite = null, Func<LocalizationOption> loadLocalization = null, bool generateEmptyLogic = false)
        {

            var defClass = InnitDefintionType(Id, overwriteVanilla);

            MakeMethod(nameof(StatusEffectTemplate.MakeConfig), makeConfig, defClass, overwriteVanilla);

            MakeMethod(nameof(StatusEffectTemplate.LoadSprite), loadSprite, defClass, overwriteVanilla);

            MakeMethod(nameof(StatusEffectTemplate.LoadLocalization), loadLocalization, defClass, overwriteVanilla);


            if (generateEmptyLogic)
            {
                MakeEntityLogic(Id, defClass, typeof(StatusEffect));
            }
        }


    }
}
