using Cysharp.Threading.Tasks;
using LBoL.ConfigData;
using LBoL.Presentation;
using LBoLEntitySideloader.Resource;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using static LBoL.Presentation.AudioManager;

namespace LBoLEntitySideloader.Entities
{
    public abstract class SfxTemplate : EntityDefinition,
        IConfigProvider<SfxConfig>,
        IResourceConsumer<List<UniTask<AudioClip>>>
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
        /// <summary>
        /// put a single sfx for concrete id => sfx mapping. Put more to make AudioManger of the sfx randomly
        /// </summary>
        /// <returns></returns>
        public abstract List<UniTask<AudioClip>> LoadSfxListAsync();

        public async void Consume(List<UniTask<AudioClip>> audioClips)
        {
            var config = SfxConfig.FromName(UniqueId);
            SfxEntry sfxEntry = null;
            if(audioClips.Count == 1)
                sfxEntry = new SfxEntry(config.Rep, config.Volume, await audioClips[0]);
            if (audioClips.Count > 1)
            {
                sfxEntry = new SfxEntry(config.Rep, config.Volume);
                foreach(var acTask in audioClips)
                    sfxEntry.Clips.Add(await acTask);
            }
            if (sfxEntry == null)
            {
                Log.log.LogError($"{this.GetType()}: failed to load any sfx AudioClips");
            }

            AudioManager.Instance._sfxTable.AlwaysAdd(UniqueId, sfxEntry);
        }
    }
}
