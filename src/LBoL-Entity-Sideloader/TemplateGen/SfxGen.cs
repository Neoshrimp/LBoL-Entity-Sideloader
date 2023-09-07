using Cysharp.Threading.Tasks;
using LBoL.ConfigData;
using LBoLEntitySideloader.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace LBoLEntitySideloader.TemplateGen
{
    public class SfxGen : TemplateGen<SfxTemplate>
    {

        public void QueueGen(IdContainer Id, bool overwriteVanilla, Func<SfxConfig> makeConfig = null, Func<List<UniTask<AudioClip>>> loadSfxListAsync = null)
        {


            var defClass = InnitDefintionType(Id, overwriteVanilla);

            MakeMethod(nameof(SfxTemplate.MakeConfig), makeConfig, defClass, overwriteVanilla);

            MakeMethod(nameof(SfxTemplate.LoadSfxListAsync), loadSfxListAsync, defClass, overwriteVanilla);


        }
    }
}
