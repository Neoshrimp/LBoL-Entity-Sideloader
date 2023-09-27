using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Presentation.Bullet;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using YamlDotNet.RepresentationModel;
using static LBoLEntitySideloader.BepinexPlugin;
using Mono.Cecil;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using LBoL.Presentation;
using System.Runtime.InteropServices;
using UnityEngine.PlayerLoop;
using Extensions.Unity.ImageLoader;
using UnityEngine.UI;
using UnityEngine.Experimental.Rendering;

namespace LBoLEntitySideloader.Resource
{

    public class ResourceLoader
    {

        public static Texture2D LoadTexture(string name, IResourceSource source)
        {
            using Stream resource = source.Load(name);

            if (resource == null)
                return null;

            using var memoryStream = new MemoryStream();
            var buffer = new byte[16384];
            int count;
            while ((count = resource!.Read(buffer, 0, buffer.Length)) > 0)
                memoryStream.Write(buffer, 0, count);
            var spriteTexture = new Texture2D(0, 0, TextureFormat.ARGB32, false)
            {
                anisoLevel = 1,
                filterMode = 0
            };

            spriteTexture.LoadImage(memoryStream.ToArray());
            return spriteTexture;
        }

        public static Sprite LoadSprite(string name, IResourceSource source, Rect? rect = null, int ppu = 1, Vector2? pivot = null)
        {
            return LoadSprite(name, source, ppu, anisoLevel: 1, filterMode: FilterMode.Point, false, rect, pivot);
        }


        public static Sprite LoadSprite(string name, IResourceSource source, int ppu, int anisoLevel, FilterMode filterMode, bool generateMipMaps = false, Rect ? rect = null, Vector2? pivot = null)
        {
            using Stream resource = source.Load(name);

            if (resource == null)
                return null;

            using var memoryStream = new MemoryStream();
            var buffer = new byte[16384];

            int count;
            while ((count = resource!.Read(buffer, 0, buffer.Length)) > 0)
                memoryStream.Write(buffer, 0, count);

            //var spriteTexture = new Texture2D(0, 0, TextureFormat.ARGB32, true)
            var texGenFlags = TextureCreationFlags.None;
            if (generateMipMaps)
                texGenFlags |= TextureCreationFlags.MipChain;
            var spriteTexture = new Texture2D(0, 0, GraphicsFormat.R8G8B8A8_SRGB, texGenFlags)
            {
                anisoLevel = anisoLevel,
                filterMode = filterMode
            };


            // can't be used on background threads
            spriteTexture.LoadImage(memoryStream.ToArray());

            if (rect == null)
                rect = new Rect(0, 0, spriteTexture.width, spriteTexture.height);

            if (pivot == null) { pivot = new Vector2(0.5f, 0.5f); }
            var sprite = Sprite.Create(spriteTexture, rect.Value, (Vector2)pivot, ppu);

            //spriteTexture.Apply(true, true);
            return sprite;

        }

        public async static UniTask<Sprite> LoadSpriteAsync(string name, DirectorySource source, int ppu = 100, GraphicsFormat finalGraphicsFormat = GraphicsFormat.R8G8B8A8_SRGB, int anisoLevel = 1, FilterMode filterMode = FilterMode.Bilinear, SpriteMeshType spriteMeshType = SpriteMeshType.Tight, Rect? rect = null, Vector2? pivot = null, string protocol = "file://")
        {


            var path = "";
            if (source != null)
                path = source.FullPath(name);
            else
                path = name;

            // ImageLoader used: https://github.com/tarunkrishnat0/Unity-ImageLoader
            return await ImageLoader.LoadSpriteMemoryOptimized(protocol + path, ppu, finalGraphicsFormat, anisoLevel, filterMode, spriteMeshType, pivot, rect, ignoreImageNotFoundError: false);

        }



        public static YamlMappingNode LoadYaml(string name, IResourceSource source)
        {
            using var stream = source.Load(name);

            using var reader = new StreamReader(stream, encoding: System.Text.Encoding.UTF8);

            var text = reader.ReadToEnd();

            try
            {
                var yaml = Localization.ParseYaml(text);
                return yaml;
            }
            catch (Exception ex)
            {

                log.LogWarning($"illegal yaml while loading {name}:");
                log.LogWarning($"{ex}");
                return null;
            }

        }


        public static AssetBundle LoadAssetBundle(string name, DirectorySource source)
        {
            var path = "";
            if (source != null)
                path = source.FullPath(name);
            else
                path = name;
            return AssetBundle.LoadFromFile(path);
        }



        /// <summary>
        /// Uses UnityWebRequestMultimedia.GetAudioClip to read file from disk. Could use http(s):// protocol to fetch file from URL.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="audioType"></param>
        /// <param name="source"></param>
        /// <param name="protocol"></param>
        /// <returns></returns>
        //2do band-aid solution with potential bad Addressables memory management
        public async static UniTask<AudioClip> LoadAudioClip(string name, AudioType audioType, DirectorySource source, string protocol = "file://")
        {


            var path = "";
            if (source != null)
                path = source.FullPath(name);
            else
                path = name;


            Log.LogDevExtra()?.LogInfo($"Loading audio from {path}");
            using var uwr = UnityWebRequestMultimedia.GetAudioClip(protocol + path, audioType);

            uwr.timeout = 20;

            await uwr.SendWebRequest();

            if (string.IsNullOrEmpty(uwr.error))
            {
                // DownloadHandlerAudioClip.GetContent is slow and needs to be awaited
                var clip = await UniTask.RunOnThreadPool(() => DownloadHandlerAudioClip.GetContent(uwr));
                return clip;
            }
            else
            {
                log.LogError(uwr.error);
                return null;
            }

        }





        public static byte[] ResourceBinary(string name, IResourceSource source)
        {
            using var stream = source.Load(name);

            if (stream == null) return null;
            byte[] ba = new byte[stream.Length];
            stream.Read(ba, 0, ba.Length);
            return ba;
        }
    }
}

