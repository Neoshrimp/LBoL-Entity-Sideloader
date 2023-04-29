using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace LBoLEntitySideloader.Resource
{
    public class ExhibitSprites : IResourceProvider<Sprite>
    {
        /// <summary>
        /// 512x512 image
        /// </summary>
        public Sprite main;


        /// <summary>
        /// [optional] alternative image when some condition has triggered. Override OverrideIconName method to specify the condition, example, Mokou's Lighter LBoL.EntityLib.Exhibits.Adventure.Dahuoji.
        /// </summary>
        public Sprite overrideSprite;

        /// <summary>
        /// [optional] suffix used to specify alternative icon key. The resulting key will be UniqueId + overrideSuffix and will need to be specified in OverrideIconName.
        /// </summary>
        public string overrideSuffix = "Override";

        /// <summary>
        /// [optional] additional sprites to cache to ResourcesHelper.Sprites dictionary. Requires custom patches to make use of. UniqueId will be prefixed for each of the keys in this dictionary when caching.
        /// </summary>
        public Dictionary<string, Sprite> customSprites = new Dictionary<string, Sprite>();

        public ExhibitSprites() { }

        public ExhibitSprites(Sprite main)
        {
            this.main = main;
        }

        public ExhibitSprites(Sprite main, Sprite overrideSprite, string overrideSuffix = "Override") : this(main)
        {
            this.overrideSprite = overrideSprite;
            this.overrideSuffix = overrideSuffix;
        }

        public Sprite Load()
        {
            return main;
        }

        public Dictionary<string, Sprite> LoadMany()
        {
            var dic = new Dictionary<string, Sprite> { { overrideSuffix, overrideSprite } };

            customSprites.Do(kv => dic.TryAdd(kv.Key, kv.Value));

            return dic;
        }
    }
}
