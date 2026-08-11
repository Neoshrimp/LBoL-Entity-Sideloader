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
        IResourceConsumer<LocalizationOption>,
        IResourceConsumer<AdventureImages>,
        IResourceConsumer<YarnData>
    {
        public override Type ConfigType() => typeof(AdventureConfig);
        public override Type TemplateType() => typeof(AdventureTemplate);
        public override Type EntityType() => typeof(Adventure);

        /// <summary>
        /// No: 0,
        /// Id: GetId(),
        /// HostId: Id of the character in the event. Will try to find an enemyunit with the same Id to use as host,
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
                    Id: GetId(),
                    HostId: "",
                    HostId2: "",
                    Music: 0,
                    HideUlt: false,
                    TempArt: false
                );
        }
        public abstract AdventureConfig MakeConfig();

        public abstract LocalizationOption LoadLocalization();
        public void Consume(LocalizationOption resource)
        {
            ProcessLocalization(resource, EntityType());
        }


        /// <summary>
        /// Loads adventure images (main event image, etc.).
        /// Return null if no custom images are needed.
        /// Example:
        /// ```
        /// var imgs = new AdventureImages();
        /// imgs.AutoLoad(this, BepinexPlugin.embeddedSource);
        ///    return imgs;
        /// ```
        /// Check AdventureImages for more info.
        /// </summary>
        public abstract AdventureImages LoadAdventureImages();

        public void Consume(AdventureImages resource)
        {
            if (resource == null) return;

            if (resource.main != null)
            {
                AdventureRegistry.RegisterAdventureImage(UniqueId, resource.main);
            }

            foreach (var kv in resource.extraImages)
            {
                if (kv.Value != null)
                {
                    AdventureRegistry.RegisterAdventureImage(kv.Key, kv.Value);
                }
            }
        }


        /// <summary>
        /// Loads compiled Yarn binary (.yarnc) and dialogue localization YAMLs.
        /// Example:
        /// ```
        /// var yarnData = new YarnData();
        /// yarnData.AutoLoad(this, BepinexPlugin.embeddedSource, BepinexPlugin.directorySource);
        /// return yarnData;
        /// ```
        /// This will load the yarn from resources and the yaml from directory.
        /// </summary>
        public abstract YarnData LoadYarnData();

        public void Consume(YarnData resource)
        {
            if (resource == null) return;
            AdventureRegistry.RegisterYarnData(UniqueId, resource);
        }
    }
}
