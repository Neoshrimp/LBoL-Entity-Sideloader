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
    public abstract class UiSoundTemplate : EntityDefinition,
        IConfigProvider<UiSoundConfig>,
        IResourceConsumer<List<UniTask<AudioClip>>>

    {
        public override Type ConfigType() => typeof(UiSoundConfig);

        public override Type EntityType() => throw new InvalidDataException();

        public override Type TemplateType() => typeof(UiSoundTemplate);
        public UiSoundConfig DefaultConfig()
        {
            var config = new UiSoundConfig(
                    Name: "",
                    Folder: "",
                    Path: "",
                    Volume: 1f
                );
            return config;
        }

        public abstract UiSoundConfig MakeConfig();

        public abstract List<UniTask<AudioClip>> LoadSfxListAsync();


        public async void Consume(List<UniTask<AudioClip>> audioClips)
        {
            var config = UiSoundConfig.FromName(UniqueId);
            UiEntry uiEntry = null;
            if (audioClips.Count == 1)
                uiEntry = new UiEntry(await audioClips[0], config.Volume);
            if (audioClips.Count > 1)
            {
                uiEntry = new UiEntry(config.Volume);
                foreach (var acTask in audioClips)
                    uiEntry.Clips.Add(await acTask);
            }
            if (uiEntry == null)
            {
                Log.log.LogError($"{this.GetType()}: failed to load any ui AudioClips");
            }

            AudioManager.Instance._uiTable.AlwaysAdd(UniqueId, uiEntry);
        }
    }
}
