using LBoL.ConfigData;
using LBoLEntitySideloader.Resource;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Linq;
using HarmonyLib;
using LBoL.Presentation;

namespace LBoLEntitySideloader.Entities
{
    public abstract class BgmTemplate : EntityDefinition,
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




/*        [HarmonyPatch(typeof(ResourcesHelper), nameof(ResourcesHelper.LoadBgmAsync))]
        class BgmLoad_Patch
        {
            static bool Prefix(string path)
            {

            }
        }*/



    }

}
