using BepInEx;
using HarmonyLib;
using LBoLEntitySideloader;
using LBoLEntitySideloader.Resource;
using System.Reflection;
using UnityEngine;


namespace GoodExamples
{
    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.version)]
    [BepInDependency(LBoLEntitySideloader.PluginInfo.GUID, BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency(AddWatermark.API.GUID, BepInDependency.DependencyFlags.SoftDependency)]
    [BepInProcess("LBoL.exe")]
    public class BepinexPlugin : BaseUnityPlugin
    {

        private static readonly Harmony harmony = PluginInfo.harmony;

        internal static BepInEx.Logging.ManualLogSource log;

        // sequence table for easily keeping track of indexes. Accepts either Template or Config types as arguments
        internal static TemplateSequenceTable sequenceTable = new TemplateSequenceTable();

        // source from where the assets will be loaded. Can have several sources 
        // but for now EmbeddedSource is the best option.
        // When using vs project template any files or folders put into Resource directory
        // will be considered embedded and will be packaged in plugin's dll 
        internal static IResourceSource embeddedSource = new EmbeddedSource(Assembly.GetExecutingAssembly());


        private void Awake()
        {
            log = Logger;

            // very important. Without this the entry point MonoBehaviour gets destroyed
            DontDestroyOnLoad(gameObject);
            gameObject.hideFlags = HideFlags.HideAndDontSave;


            // important for Sideloader. Notifies the Sideloader that this mod is using Sideloader
            EntityManager.RegisterSelf();

            // for optional Harmony patches
            harmony.PatchAll();

            // activates watermark text if watermark plugin is loaded
            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(AddWatermark.API.GUID))
                WatermarkWrapper.ActivateWatermark();
        }

        private void OnDestroy()
        {
            if (harmony != null)
                harmony.UnpatchSelf();
        }


    }
}
