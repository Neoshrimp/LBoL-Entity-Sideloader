using Cysharp.Threading.Tasks;
using HarmonyLib;
using LBoL.Core;
using LBoL.Core.Dialogs;
using LBoL.Presentation;
using LBoL.Presentation.UI;
using LBoLEntitySideloader.Resource;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Yarn;

namespace LBoLEntitySideloader
{
    public static class AdventureRegistry
    {
        // Registered yarn programs: dialogName (e.g. "Adventure/YuukaGarden" or "YuukaGarden") -> YarnData
        internal static readonly Dictionary<string, YarnData> YarnPrograms = new Dictionary<string, YarnData>(StringComparer.OrdinalIgnoreCase);

        // Registered adventure textures: imageName -> Texture2D
        internal static readonly Dictionary<string, Texture2D> AdventureTextures = new Dictionary<string, Texture2D>(StringComparer.OrdinalIgnoreCase);

        public static void RegisterYarnData(string dialogName, YarnData yarnData)
        {
            if (yarnData == null) return;

            YarnPrograms[dialogName] = yarnData;
            // Also store with "Adventure/" prefix if not present to ensure lookup works regardless of caller format
            if (!dialogName.StartsWith("Adventure/", StringComparison.OrdinalIgnoreCase))
            {
                YarnPrograms["Adventure/" + dialogName] = yarnData;
            }
        }

        public static void RegisterAdventureImage(string imageName, Texture2D texture)
        {
            if (texture != null)
            {
                AdventureTextures[imageName] = texture;
            }
        }
    }

    [HarmonyPatch(typeof(DialogRunner), "LoadAsync")]
    public static class DialogRunnerPatch
    {
        static bool Prefix(string name, IVariableStorage storage, Yarn.Library library, ref UniTask<DialogRunner> __result)
        {
            BepinexPlugin.log.LogInfo($"[DialogRunnerPatch] Intercepted LoadAsync for dialog name: '{name}'");

            if (!AdventureRegistry.YarnPrograms.TryGetValue(name, out YarnData yarnData) || yarnData == null)
            {
                BepinexPlugin.log.LogWarning($"[DialogRunnerPatch] Dialog '{name}' NOT found in AdventureRegistry. Skipping patch.");
                return true;
            }

            if (yarnData.compiledBytes == null)
            {
                BepinexPlugin.log.LogError($"[DialogRunnerPatch] Dialog '{name}' found in registry, but compiledBytes is NULL!");
                return true;
            }

            BepinexPlugin.log.LogInfo($"[DialogRunnerPatch] Found YarnData for '{name}'. Getting string table...");
            Dictionary<string, string> stringTable = yarnData.GetStringTableForCurrentLocale();

            ConstructorInfo ctor = typeof(DialogRunner).GetConstructor(
                BindingFlags.NonPublic | BindingFlags.Instance,
                null,
                new[] { typeof(string), typeof(byte[]), typeof(IDictionary<string, string>), typeof(IVariableStorage), typeof(Yarn.Library) },
                null);

            var runner = (DialogRunner)ctor.Invoke(new object[] { name, yarnData.compiledBytes, stringTable, storage, library });

            __result = UniTask.FromResult(runner);
            BepinexPlugin.log.LogInfo($"[DialogRunnerPatch] Successfully created DialogRunner for '{name}' with {stringTable.Count} string table entries.");
            return false;
        }
    }

    [HarmonyPatch(typeof(ResourcesHelper), nameof(ResourcesHelper.LoadAdventureImage))]
    public static class AdventureImagePatch
    {
        static bool Prefix(string name, ref Texture2D __result)
        {
            if (AdventureRegistry.AdventureTextures.TryGetValue(name, out Texture2D texture) && texture != null)
            {
                __result = texture;
                return false; // Skip vanilla Addressables load
            }
            return true;
        }
    }
}