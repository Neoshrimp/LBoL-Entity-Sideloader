using LBoL.ConfigData;
using LBoL.Core.Adventures;
using LBoLEntitySideloader.Resource;
using System;
using System.Collections.Generic;
using System.Text;

namespace LBoLEntitySideloader.Entities
{
    public abstract class AdventureTemplate : EntityDefinition,
        IConfigProvider<AdventureConfig>,
        IAdventureProvider<Adventure>,
        IResourceConsumer<LocalizationOption>
    {
        public override Type ConfigType() => typeof(AdventureConfig);
        public override Type TemplateType() => typeof(AdventureTemplate);
        public override Type EntityType() => typeof(Adventure);

        /// <summary>
        /// No: 0,
        /// Id: "",
        /// HostId: "",
        /// HostId2: "",
        /// Music: play specific event bgm with id "Adventure" + {music},
        /// HideUlt: false,
        /// TempArt: false
        /// </summary>
        /// <returns></returns>
        public AdventureConfig DefaultConfig()
        {
            return new AdventureConfig(
                    No: 0,
                    Id: "",
                    HostId: "",
                    HostId2: "",
                    Music: 0,
                    HideUlt: false,
                    TempArt: false
                );
        }
        public abstract AdventureConfig MakeConfig();

        public void Consume(LocalizationOption resource)
        {
            throw new NotImplementedException();
        }
    }
}
