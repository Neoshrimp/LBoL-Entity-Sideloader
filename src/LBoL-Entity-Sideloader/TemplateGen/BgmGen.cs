using Cysharp.Threading.Tasks;
using LBoL.ConfigData;
using LBoL.Core.StatusEffects;
using LBoLEntitySideloader.Entities;
using LBoLEntitySideloader.Resource;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace LBoLEntitySideloader.TemplateGen
{
    public class BgmGen : TemplateGen<BgmTemplate>
    {

        public void QueueGen(IdContainer Id, bool overwriteVanilla, Func<BgmConfig> makeConfig = null, Func<UniTask<AudioClip>> loadAudioClipAsync = null)
        {


            var defClass = InnitDefintionType(Id, overwriteVanilla);

            MakeMethod(nameof(BgmTemplate.MakeConfig), makeConfig, defClass, overwriteVanilla);

            MakeMethod(nameof(BgmTemplate.LoadAudioClipAsync), loadAudioClipAsync, defClass, overwriteVanilla);


        }
    }
}
