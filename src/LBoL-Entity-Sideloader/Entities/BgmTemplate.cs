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
                    Show: true,
                    Name: "",
                    Folder: "",
                    Path: "",
                    LoopStart: null,
                    LoopEnd: null,
                    TrackName: "",
                    Artist: "",
                    Original: "",
                    Comment: ""
            );

            return config;
        }

        public abstract BgmConfig MakeConfig();

        public abstract AudioInfo LoadAudioClip();


        //2do INCLUDE audio lib with sideloader

        [HarmonyPatch(typeof(ResourcesHelper), nameof(ResourcesHelper.LoadBgmAsync))]
        //[HarmonyDebug]
        internal class BgmLoad_Patch
        {
            // path = ID
            static bool Prefix(string path, ref BgmTemplate __state)
            {

                __state = null;

                var Id = path.TrimStart('/');
                if (UniqueTracker.Instance.IsLoadedOnDemand(typeof(BgmTemplate), Id, out var entityDefinition))
                {
                    Log.log.LogDebug("TO load custom bgm");
                    if (entityDefinition is BgmTemplate bt)
                    {
                        __state = bt;
                        return false;

                    }
                    Log.log.LogError($"Loading custom bgm: {entityDefinition.GetType().Name} was not of {typeof(BgmTemplate)} type but of {entityDefinition.TemplateType().Name} type instead");
                    return true;
                }

                //Log.log.LogDebug("vanilla bgm");
                return true;
            }

            static void Postfix(ref UniTask<AudioClip> __result, ref BgmTemplate __state)
            {
                Log.log.LogDebug("bgm load postfix");

                if (__state != null)
                {
                    //Log.log.LogDebug("loadING custom bgm");

                    var bt = __state;

                    __result = UniTask.RunOnThreadPool(() => bt.LoadAudioClip().main);
                }
            }
        }



    }

}
