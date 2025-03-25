using LBoL.ConfigData;
using LBoLEntitySideloader.Resource;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Linq;
using HarmonyLib;
using LBoL.Presentation;
using Cysharp.Threading.Tasks;
using UnityEngine;
using LBoL.Base.Extensions;
using UnityEngine.AddressableAssets;
using YamlDotNet.Core.Tokens;

namespace LBoLEntitySideloader.Entities
{
    public abstract class BgmTemplate : EntityDefinition,
        // 2do maybe add resource provider interface
            IConfigProvider<BgmConfig>
    {

        public override Type TemplateType()
        {
            return typeof(BgmTemplate);
        }

        public override Type ConfigType()
        {
            return typeof(BgmConfig);
        }


        public override Type EntityType()
        {
            throw new InvalidDataException();
        }

        public BgmConfig DefaultConfig()
        {
            var config = new BgmConfig(
                    ID: "",
                    No: 0,
                    Name: "",
                    Folder: "",
                    Path: "",
                    Volume : 1f,
                    LoopStart: null,
                    LoopEnd: null,
                    ExtraDelay: null,
                    TrackName: "",
                    Artist: "",
                    Original: "",
                    Comment: ""
            );

            return config;
        }

        public abstract BgmConfig MakeConfig();

        public abstract UniTask<AudioClip> LoadAudioClipAsync();


        [HarmonyPatch(typeof(ResourcesHelper), nameof(ResourcesHelper.LoadBgmAsync))]
        internal class BgmLoad_Patch
        {
            // path = ID
            static bool Prefix(string path, ref UniTask<AudioClip> __result)
            {

                var Id = path.TrimStart('/');
                //Log.log.LogDebug($"bgm to load: {Id}");
                if (UniqueTracker.Instance.IsLoadedOnDemand(typeof(BgmTemplate), Id, out var entityDefinition))
                {

                    if (entityDefinition is BgmTemplate bt && EntityManager.HandleOverwriteWrap(() => { }, bt, nameof(LoadAudioClipAsync), bt.user))
                    {
                        //Log.log.LogDebug($"custom bgm");
                        __result = bt.LoadAudioClipAsync();
                        return false;

                    }
                    return true;
                }

                //Log.log.LogDebug($"vanilla bgm");
                return true;
            }

/*            static async void Postfix(UniTask<AudioClip> __result)
            {
                Log.log.LogDebug("bgm load postfix");

                // double await = bad
                var rez = await UniTask.WhenAll(__result );

                Log.log.LogDebug("postfix after await");
                var clip = rez.TryGetValue(0);

                if (clip == null)
                {
                    // potential fallback
                }

            }*/
        }



    }

}
