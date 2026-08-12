using LBoLEntitySideloader.Entities;
using System.Collections.Generic;
using UnityEngine;

namespace LBoLEntitySideloader.Resource
{
    public class AdventureImages
    {
        public Texture2D main;
        /// <summary>
        /// Any additional image needed for the event.
        /// </summary>
        public Dictionary<string, Texture2D> extraImages = new Dictionary<string, Texture2D>();

        public AdventureImages() { }

        public AdventureImages(Texture2D main)
        {
            this.main = main;
        }

        /// <summary>
        /// Automatically loads {AdventureId}.png as main,
        /// and any extra suffixes provided (e.g. extraSuffixes: "_angry", "_happy") 
        /// will be loaded as {AdventureId}{Suffix}.png.
        /// Example to use in a yarn:
        /// <<setAdventureImage "">>          
        /// <<setAdventureImage "_angry">>    
        /// <<crossfadeAdventureImage "_happy" 0.5>> 
        /// </summary>
        /// <param name="adventureTemplate">this</param>
        /// <param name="source">Either BepinexPlugin.embeddedSource (if you want it to be from Resources) or .directorySource (If you want it to be from DirResources, aka exposed outside the dll)</param>
        /// <param name="extension">Extension of your image. For those who want their bmp images.</param>
        /// <param name="extraSuffixes">Extra images for the event. For example, "_angry" for the image "Adventure_angry.png".</param>
        public void AutoLoad(AdventureTemplate adventureTemplate, IResourceSource source, string extension = ".png", params string[] extraSuffixes)
        {
            string id = adventureTemplate.GetId();

            // 1. Load main texture (registered as "YuukaGarden")
            main = ResourceLoader.LoadTexture(id + extension, source);

            // 2. Load extra suffix textures if specified
            if (extraSuffixes != null)
            {
                foreach (string suffix in extraSuffixes)
                {
                    string extraImageName = id + suffix; // e.g. "YuukaGarden_angry"
                    Texture2D tex = ResourceLoader.LoadTexture(extraImageName + extension, source);

                    if (tex != null)
                    {
                        extraImages[extraImageName] = tex;
                    }
                    else
                    {
                        Log.log.LogWarning($"[AdventureImages] Failed to load extra image '{extraImageName}' from source.");
                    }
                }
            }
        }
    }
}