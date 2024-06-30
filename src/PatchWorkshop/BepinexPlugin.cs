using BepInEx;
using HarmonyLib;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.StatusEffects;
using LBoL.EntityLib.EnemyUnits.Normal.Bats;
using LBoL.Presentation;
using LBoLEntitySideloader;
using LBoLEntitySideloader.CustomHandlers;
using LBoLEntitySideloader.Entities;
using LBoLEntitySideloader.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;


namespace PatchWorkshop
{
    [BepInPlugin(PatchWorkshop.PInfo.GUID, PatchWorkshop.PInfo.Name, PatchWorkshop.PInfo.version)]
    [BepInDependency(LBoLEntitySideloader.PluginInfo.GUID, BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency(AddWatermark.API.GUID, BepInDependency.DependencyFlags.SoftDependency)]
    [BepInProcess("LBoL.exe")]
    public class BepinexPlugin : BaseUnityPlugin
    {

        private static readonly Harmony harmony = PatchWorkshop.PInfo.harmony;

        internal static BepInEx.Logging.ManualLogSource log;

        internal static TemplateSequenceTable sequenceTable = new TemplateSequenceTable();

        internal static IResourceSource embeddedSource = new EmbeddedSource(Assembly.GetExecutingAssembly());

        // add this for audio loading
        internal static DirectorySource directorySource = new DirectorySource(PatchWorkshop.PInfo.GUID, "");

        internal static BatchLocalization exBatchLoc = new BatchLocalization(directorySource, typeof(ExhibitTemplate), "Exhibits");

        public static BepinexPlugin Instance;

        private void Awake()
        {
            log = Logger;
            Instance = this;

            // very important. Without this the entry point MonoBehaviour gets destroyed
            DontDestroyOnLoad(gameObject);
            gameObject.hideFlags = HideFlags.HideAndDontSave;

            EntityManager.RegisterSelf();

            harmony.PatchAll();

            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(AddWatermark.API.GUID))
                WatermarkWrapper.ActivateWatermark();

            // event is registered at the start/load of a gameRun
            CHandlerManager.RegisterGameEventHandler(
                (GameRunController gr) => gr.DeckCardsAdded,
                CustomHandlers.LogCardName
                );

            // dublicate handlers will not be added
            CHandlerManager.RegisterGameEventHandler(
                (GameRunController gr) => gr.DeckCardsAdded,
                CustomHandlers.LogCardName
                );


            CustomHandlers.RegisterAddFpOnDmg();


        }




        private void OnDestroy()
        {
            if (harmony != null)
                harmony.UnpatchSelf();
        }



    }
}
