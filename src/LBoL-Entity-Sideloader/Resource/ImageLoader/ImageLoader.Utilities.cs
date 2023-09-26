using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Extensions.Unity.ImageLoader
{
    public static class Utils
    {
        public enum SizeUnits
        {
            Byte, KB, MB, GB, TB, PB, EB, ZB, YB
        }

        public static string ToSize(Int64 value, SizeUnits unit = SizeUnits.MB)
        {
            return (value / (double)Math.Pow(1024, (Int64)unit)).ToString("0.00") + unit.ToString();
        }

        public static bool IsPowerOfTwo(int x)
        {
            return (x != 0) && ((x & (x - 1)) == 0);
        }

        public static Texture2D CreateTexWithMipmaps(byte[] data, GraphicsFormat origGraphicsFormat, int height = 4, int width = 4, TextureFormat textureFormat = default, string name = "")
        {
            GraphicsFormat finalGraphicsFormat;
#if UNITY_2021_3_OR_NEWER
            bool hasAlpha = GraphicsFormatUtility.HasAlphaChannel(origGraphicsFormat);
            if (Utils.IsPowerOfTwo(width) && Utils.IsPowerOfTwo(height) && width == height)
            {
                // Standalone - RGBA_DXT5_SRGB/RGBA_DXT1_SRGB
                // Android - RGBA_ETC2_SRGB/RGB_ETC2_SRGB
#if UNITY_ANDROID
                finalGraphicsFormat = hasAlpha ? GraphicsFormat.RGBA_ETC2_SRGB : GraphicsFormat.RGB_ETC2_SRGB;
#else
                finalGraphicsFormat = hasAlpha ? GraphicsFormat.RGBA_DXT5_SRGB : GraphicsFormat.RGBA_DXT1_SRGB;
#endif
            }
            else
            {
                // graphicsFormat = hasAlpha ? GraphicsFormat.R8G8B8A8_SRGB : GraphicsFormat.R8G8B8_SRGB;
                finalGraphicsFormat = GraphicsFormat.R8G8B8A8_SRGB;
            }
            // Debug.Log($"Format {finalGraphicsFormat} supported = " + SystemInfo.IsFormatSupported(finalGraphicsFormat, FormatUsage.Linear));
#else
            finalGraphicsFormat = GraphicsFormat.RGBA_DXT5_SRGB;
#endif
            
            TextureCreationFlags flags = TextureCreationFlags.MipChain;
            var loadedTexture = new Texture2D(width, height, finalGraphicsFormat, flags);
            // var loadedTexture = new Texture2D(width, height, textureFormat, true); // Generates mipmaps without compression
            loadedTexture.wrapMode = TextureWrapMode.Clamp;

            //try
            //{
            //    loadedTexture.LoadImage(data);
            //} catch (Exception e)
            //{
            //    Debug.Log($"CreateTexWithMipmaps: LoadImage failed error={e.Message}, using format={finalGraphicsFormat}, trying again with {GraphicsFormat.R8G8B8A8_SRGB}");
            //    finalGraphicsFormat = GraphicsFormat.R8G8B8A8_SRGB;
            //    loadedTexture = new Texture2D(width, height, finalGraphicsFormat, flags);
            //    if (loadedTexture.LoadImage(data) == false)
            //    {
            //        Debug.LogError($"CreateTexWithMipmaps: LoadImage failed, using format={finalGraphicsFormat}, returning null texture");
            //        loadedTexture = null;
            //    }
            //}

            if (loadedTexture.LoadImage(data) == false)
            {
                Debug.Log($"CreateTexWithMipmaps: LoadImage failed, using format={finalGraphicsFormat}, trying again with {GraphicsFormat.R8G8B8A8_SRGB}");
                finalGraphicsFormat = GraphicsFormat.R8G8B8A8_SRGB;
                loadedTexture = new Texture2D(width, height, finalGraphicsFormat, flags);
                if (loadedTexture.LoadImage(data) == false)
                {
                    Debug.LogError($"CreateTexWithMipmaps: LoadImage failed, using format={finalGraphicsFormat}, returning null texture");
                    loadedTexture = null;
                }
            }

            return loadedTexture;
        }

        public static Texture2D CreateTexWithMipmaps(byte[] data, bool shouldGenerateMipMaps = true, string name = "DynamicTex")
        {
            GraphicsFormat finalGraphicsFormat = GraphicsFormat.R8G8B8A8_SRGB;
//#if UNITY_ANDROID
//            finalGraphicsFormat = GraphicsFormat.R8G8B8A8_SRGB;
//            //TextureFormat textureFormat = TextureFormat.ETC2_RGB;
//#else
//            finalGraphicsFormat = GraphicsFormat.RGBA_DXT5_SRGB;
//#endif

            TextureCreationFlags flags = TextureCreationFlags.None;
            if (shouldGenerateMipMaps)
                flags = TextureCreationFlags.MipChain;

            // Debug.Log($"Format {finalGraphicsFormat} supported = " + SystemInfo.IsFormatSupported(finalGraphicsFormat, FormatUsage.Linear));
            // Debug.Log($"Format {textureFormat} supported = " + SystemInfo.SupportsTextureFormat(textureFormat));

            var loadedTexture = new Texture2D(4, 4, finalGraphicsFormat, flags) { 
                name = name+"-"+shouldGenerateMipMaps,
                wrapMode = TextureWrapMode.Clamp,
            };
            //var loadedTexture = new Texture2D(4, 4, textureFormat, true); // Generates mipmaps without compression
            // loadedTexture.wrapMode = TextureWrapMode.Clamp;

            if (loadedTexture.LoadImage(data) == false)
            {
                Debug.Log($"CreateTexWithMipmaps: LoadImage failed using format={finalGraphicsFormat}, size={loadedTexture.width}x{loadedTexture.height} trying again with {GraphicsFormat.R8G8B8A8_SRGB}");
                //finalGraphicsFormat = GraphicsFormat.R8G8B8A8_SRGB;
                //loadedTexture = new Texture2D(4, 4, finalGraphicsFormat, flags);
                //if (loadedTexture.LoadImage(data) == false)
                //{
                //    Debug.LogError($"CreateTexWithMipmaps: LoadImage failed, using format={finalGraphicsFormat}, returning null texture");
                //    loadedTexture = null;
                //}
            }

            if(loadedTexture.width % 4 == 0 && loadedTexture.height % 4 == 0)
                loadedTexture.Compress(false);

            return loadedTexture;
        }

        public static IEnumerator GetImageUsingUWRTexture(string url, Image img = null)
        {
            UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(url);

            yield return uwr.SendWebRequest();

            string name = Path.GetFileNameWithoutExtension(url);
            if (uwr.result != UnityWebRequest.Result.Success || !uwr.isDone)
            {
                Debug.Log($"GetImageUsingUWRTexture: Download failed : name={name}, error=" + uwr.error);
            }
            else
            {
                Texture2D uwrTexture = ((DownloadHandlerTexture)uwr.downloadHandler).texture;

                Sprite downloadedSprite = Sprite.Create(
                    uwrTexture,
                    new Rect(0, 0, uwrTexture.width, uwrTexture.height),
                    Vector2.zero,
                    100f,
                    0,
                    SpriteMeshType.FullRect);

                if(img != null) img.overrideSprite = downloadedSprite;

                Debug.Log($"GetImageUsingUWRTexture: name={name}, RawSize={Utils.ToSize(uwrTexture.GetRawTextureData().Length)}, mipmap={uwrTexture.mipmapCount}, format={uwrTexture.format}, graphicsFormat={uwrTexture.graphicsFormat}, dimensions={uwrTexture.width}x{uwrTexture.height}");

                uwr.Dispose();
            }
        }

        public class SpriteContainer
        {
            public Sprite sprite;
        }

        public static IEnumerator GetImageUsingUWRBufferAndEnableMipmapsAndCompression(string url, SpriteContainer sprite = null)
        {
            UnityWebRequest uwr = new UnityWebRequest(url);
            uwr.downloadHandler = new DownloadHandlerBuffer();

            yield return uwr.SendWebRequest();

            string name = Path.GetFileNameWithoutExtension(url);
            if (uwr.result != UnityWebRequest.Result.Success || !uwr.isDone)
            {
                Debug.Log($"GetImageUsingUWRBufferAndEnableMipmapsAndCompression: Download failed : name={name}, error=" + uwr.error);
            }
            else
            {
                GraphicsFormat graphicsFormat = GraphicsFormat.R8G8B8A8_SRGB;
                TextureCreationFlags flags = TextureCreationFlags.None;
                // Generate MipMaps
                flags = TextureCreationFlags.MipChain;

                var createdTexture = new Texture2D(4, 4, graphicsFormat, flags)
                {
                    name = name,
                    wrapMode = TextureWrapMode.Clamp,
                };

                if (createdTexture.LoadImage(uwr.downloadHandler.data) == false)
                {
                    Debug.LogError($"GetImageUsingUWRBufferAndEnableMipmapsAndCompression: LoadImage failed using format={graphicsFormat}");
                }

                if (createdTexture.width % 4 == 0 && createdTexture.height % 4 == 0)
                    createdTexture.Compress(false);

                Sprite downloadedSprite = Sprite.Create(
                    createdTexture,
                    new Rect(0, 0, createdTexture.width, createdTexture.height),
                    Vector2.zero,
                    100f,
                    0,
                    SpriteMeshType.FullRect);

                if (sprite != null) sprite.sprite = downloadedSprite;

                Debug.Log($"GetImageUsingUWRBufferAndEnableMipmapsAndCompression: name={name}, RawSize={Utils.ToSize(createdTexture.GetRawTextureData().Length)}, mipmap={createdTexture.mipmapCount}, format={createdTexture.format}, graphicsFormat={createdTexture.graphicsFormat}, dimensions={createdTexture.width}x{createdTexture.height}");

                uwr.Dispose();
            }
        }
    }
}
