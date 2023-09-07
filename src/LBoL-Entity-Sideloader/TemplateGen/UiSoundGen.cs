using Cysharp.Threading.Tasks;
using LBoL.ConfigData;
using LBoLEntitySideloader.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace LBoLEntitySideloader.TemplateGen
{
    public class UiSoundGen : TemplateGen<UiSoundTemplate>
    {
        public void QueueGen(IdContainer Id, bool overwriteVanilla, Func<UiSoundConfig> makeConfig = null, Func<List<UniTask<AudioClip>>> loadSfxListAsync = null)
        {


            var defClass = InnitDefintionType(Id, overwriteVanilla);

            MakeMethod(nameof(UiSoundTemplate.MakeConfig), makeConfig, defClass, overwriteVanilla);

            MakeMethod(nameof(UiSoundTemplate.LoadSfxListAsync), loadSfxListAsync, defClass, overwriteVanilla);


        }

    }
}
