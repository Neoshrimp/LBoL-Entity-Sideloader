using HarmonyLib;
using LBoL.Core.SaveData;
using LBoL.Core;
using LBoL.Presentation;
using System;
using System.Text;
using YamlDotNet.Serialization;
using System.IO;

namespace LBoLEntitySideloader.PersistentValues
{
    [HarmonyPatch(typeof(GameRunController), nameof(GameRunController.Save))]
    class GameRunController_Save_Patch
    {

        static void Postfix(GameRunController __instance)
        {
            // 2do maybe. Optimize memory usage by not storing container values statically.
            var customData = UniqueTracker.Instance.customGrSaveData;
            foreach (var id in customData.Keys)
            {
                var csd = customData[id];
                csd.Save(__instance);

                GameMaster.WriteSaveData(id.GetFileName(GameMaster.Instance.CurrentSaveIndex.Value), SaveDataHelper.Serialize(csd, false));

            }
        }
    }


    [HarmonyPatch(typeof(GameRunController), nameof(GameRunController.Restore))]
    class GameRunController_Restore_Patch
    {

        static void Postfix(GameRunController __result)
        {
            var customData = UniqueTracker.Instance.customGrSaveData;

            foreach (var id in customData.Keys)
            {

                var csd = customData[id];

                var fileName = id.GetFileName(GameMaster.Instance.CurrentSaveIndex.Value);

                var filePath = Path.Combine(GameMaster.PlatformHandler.GetSaveDataFolder(), fileName);

                if (!File.Exists(filePath))
                    return;

                try
                {
                    object csdObject = new DeserializerBuilder().IgnoreUnmatchedProperties().Build().Deserialize(SaveDataHelper.DecodeYaml(File.ReadAllBytes(filePath)), csd.GetType());

                    var loadedData = (CustomGameRunSaveData)csdObject;

                    loadedData.Restore(__result);
                }
                catch (Exception ex)
                {
                    BepinexPlugin.log.LogError($"Error while loading custom save data {filePath}: {ex}");
                }
            }
        }
    }



    // custom save file deletion isn't really necessary
/*    [HarmonyPatch(typeof(GameMaster), nameof(GameMaster.TryDeleteSaveData))]
    class GameMaster_Patch
    {

        static void Postfix(string filename)
        {
            var index = GameMaster.Instance.CurrentSaveIndex;
            // if deleting game run save file..
            if (index != null && GameMaster.GetGameRunFileName(index.Value) == filename)
            {
                
            }

        }
    }*/




}
