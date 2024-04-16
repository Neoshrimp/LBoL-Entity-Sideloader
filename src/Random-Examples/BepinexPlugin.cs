using BepInEx;
using BepInEx.Configuration;
using Extensions.Unity.ImageLoader;
using HarmonyLib;
using LBoL.Core;
using LBoL.Core.Stations;
using LBoL.EntityLib.Stages.NormalStages;
using LBoL.Presentation.UI;
using LBoL.Presentation.UI.Panels;
using LBoLEntitySideloader;
using LBoLEntitySideloader.Entities;
using LBoLEntitySideloader.ReflectionHelpers;
using LBoLEntitySideloader.Resource;
using LBoLEntitySideloader.TemplateGen;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Random_Examples
{
    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.version)]
    [BepInDependency(LBoLEntitySideloader.PluginInfo.GUID, BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency(AddWatermark.API.GUID, BepInDependency.DependencyFlags.SoftDependency)]
    [BepInProcess("LBoL.exe")]
    public class BepinexPlugin : BaseUnityPlugin
    {

        private static readonly Harmony harmony = PluginInfo.harmony;

        internal static BepInEx.Logging.ManualLogSource log;

        internal static IResourceSource embeddedSource = new EmbeddedSource(Assembly.GetExecutingAssembly());

        // add this for audio loading
        internal static DirectorySource directorySource = new DirectorySource(PluginInfo.GUID, "");

        internal static TemplateSequenceTable sequenceTable = new TemplateSequenceTable(12000);

        internal static JadeBoxGen jadeBoxGen = new JadeBoxGen();

        internal static ExhibitGen exhibitGen = new ExhibitGen();

        internal static AssetBundle suikaAB;

        internal static AssetBundle effectsAB;

        internal static BatchLocalization UnitModelBatchLoc = new BatchLocalization(embeddedSource, typeof(UnitModelTemplate), Locale.En, "UnitModelEn");


        private void Awake()
        {
            log = Logger;

            // very important. Without this the entry point MonoBehaviour gets destroyed
            DontDestroyOnLoad(gameObject);
            gameObject.hideFlags = HideFlags.HideAndDontSave;


            EntityManager.RegisterSelf();

            try
            {
                harmony.PatchAll();
            }
            catch (System.Exception ex)
            {
                log.LogError(ex);
            }

            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(AddWatermark.API.GUID))
                WatermarkWrapper.ActivateWatermark();

            (new SuikaGrData()).RegisterSelf(PluginInfo.GUID);

            //2do doesnt work in 1.4 for some reason
            //JadeBoxExamples.GenJadeBoxes();

            /*            EntityManager.AddPostLoadAction(() =>
                        {
                            for (int i = 0; i < 14 * 4; i++)
                            {
                                var id = "NilTestExhibit" + i.ToString();
                                exhibitGen.QueueGen(
                                    Id: id,
                                    overwriteVanilla: false,
                                    makeConfig: () => new LunarVeilDef().DefaultConfig(),
                                    loadSprite: null,
                                    loadLocalization: GenHelper.WrapFunc((int i) => new DirectLocalization(new Dictionary<string, object>()
                                    {
                                        { "Name", "Deez" + i.ToString() },
                                        { "Description", "nuts" }
                                    }), i),
                                    generateEmptyLogic: true);
                            }
                            exhibitGen.FinalizeGen();
                        });*/



            CustomFormations.AddFormations();

            CustomLoadouts.AddLoadouts();

            NewBackgrounds.AddNewBackgrounds();


            EnemyUnitTemplate.AddBossNodeIcon("Reimu", () => ResourceLoader.LoadSprite("SelectionCircleIcon.png", SuikaPlayerDef.dir));

            //StageExamples.AddStages();

            // unload asset bundles OnDestroy
            suikaAB = ResourceLoader.LoadAssetBundle("suikaBundle", SuikaPlayerDef.dir);

            effectsAB = ResourceLoader.LoadAssetBundle("effects", directorySource);


        }


        private void OnDestroy()
        {

            if (harmony != null)
                harmony.UnpatchSelf();

            // unload asset bundles OnDestroy
            suikaAB?.Unload(false);
            effectsAB?.Unload(false);

        }



    }
}
