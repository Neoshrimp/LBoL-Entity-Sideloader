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
            using Stream resource = source.Load(name);

            if (resource == null)
                return null;

            if (pivot == null) { pivot = new Vector2(0.5f, 0.5f); }
            var assembly = Assembly.GetExecutingAssembly();
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

            // 512, 512, 
            spriteTexture.LoadImage(memoryStream.ToArray());

            if (rect == null)
                rect = new Rect(0, 0, spriteTexture.width, spriteTexture.height);

            var sprite = Sprite.Create(spriteTexture, rect.Value, (Vector2)pivot, ppu);
            return sprite;
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

        public static AudioClip LoadAudioClip(string name, IResourceSource source)
        {
            throw new NotImplementedException();
        }


        public static byte[] ResourceBinary(string name, IResourceSource source)
        {
            Assembly a = Assembly.GetExecutingAssembly();

            using var stream = source.Load(name);
            

            if (stream == null) return null;
            byte[] ba = new byte[stream.Length];
            stream.Read(ba, 0, ba.Length);
            return ba;
        }
    }
}

