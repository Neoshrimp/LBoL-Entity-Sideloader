using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

namespace LBoLEntitySideloader.Resource
{
    public class IntentionImages : IResourceProvider<Sprite>
    {
        public Sprite main;


        /// <summary>
        /// [optional] suffix => subSprite
        /// </summary>
        public Dictionary<string, Sprite> subSprites = new Dictionary<string, Sprite>();



        public Sprite Load()
        {
            return main;
        }

        public Dictionary<string, Sprite> LoadMany()
        {
            var dic = new Dictionary<string, Sprite>();
            subSprites.Do(kv => dic.TryAdd(kv.Key, kv.Value));
            return dic;
        }

    }
}