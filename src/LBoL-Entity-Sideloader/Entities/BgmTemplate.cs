using LBoL.ConfigData;
using LBoLEntitySideloader.Resource;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using static UnityEngine.UI.Image;
using System.Xml.Linq;

namespace LBoLEntitySideloader.Entities
{
    public abstract class BgmTemplate : EntityDefinition,
            IConfigProvider<BgmConfig>,
            IResourceConsumer<Audio>
    {
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

        public void Consume(Audio resource)
        {
            throw new NotImplementedException();
        }
    }

}
