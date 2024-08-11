using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

namespace LBoLEntitySideloader.Resource
{
    public class ExtraIcons : IResourceProvider<Sprite>
    {
        /// <summary>
        /// Keys should be suffixes for values returned in OverrideIconName. OverrideIconName should return entityId + suffix.
        /// </summary>
        public Dictionary<string, Sprite> icons = new Dictionary<string, Sprite>();

        public Sprite Load()
        {
            throw new System.NotImplementedException("Load not implemented for ExtraIcons");
        }

        public Dictionary<string, Sprite> LoadMany()
        {
            var dic = new Dictionary<string, Sprite>();
            icons.Do(kv => dic.Add(kv.Key, kv.Value));
            return dic;
        }
    }
}