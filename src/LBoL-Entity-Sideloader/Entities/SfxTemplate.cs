using Cysharp.Threading.Tasks;
using LBoL.ConfigData;
using LBoLEntitySideloader.Resource;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace LBoLEntitySideloader.Entities
{
    public abstract class SfxTemplate : EntityDefinition,
        IConfigProvider<SfxConfig>
    {
        public override Type ConfigType() => typeof(SfxConfig);
        public override Type EntityType() => throw new InvalidDataException();
        public override Type TemplateType() => typeof(SfxTemplate);

        /// <summary>
        /// Rep: Replay time limit
        /// </summary>
        /// <returns></returns>
        public SfxConfig DefaultConfig()
        {
            var config = new SfxConfig(
                    Name: "",
                    Folder: "",
                    Path: "",
                    Rep: 0.08,
                    Volume: 1

                );
            return config;
        }

        public abstract SfxConfig MakeConfig();

        public abstract UniTask<AudioClip> LoadSfxAsync();



    }
}
