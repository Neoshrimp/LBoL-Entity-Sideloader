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
        /// [optional] alternative images when some conditions have beenmet. Override OverrideIconName method to specify the condition, example, Mokou's Lighter LBoL.EntityLib.Exhibits.Adventure.Dahuoji. Additional sprites will cached to ResourcesHelper.Sprites dictionary. UniqueId will be prefixed for each of the keys in this dictionary when caching which will be needed when OverrideIconName tries to set a sprite
        /// </summary>
        public Dictionary<string, Sprite> customSprites = new Dictionary<string, Sprite>();

        public ExhibitSprites() { }

        public ExhibitSprites(Sprite main)
        {
            this.main = main;
        }


        public Sprite Load()
        {
            return main;
        }

        public Dictionary<string, Sprite> LoadMany()
        {
            var dic = new Dictionary<string, Sprite>();

            customSprites.Do(kv => dic.TryAdd(kv.Key, kv.Value));

            return dic;
        }
    }
}
