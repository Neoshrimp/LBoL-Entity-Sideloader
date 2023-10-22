using BepInEx;
using BepInEx.Configuration;
using Extensions.Unity.ImageLoader;
using HarmonyLib;
using LBoL.ConfigData;
using LBoL.EntityLib.Exhibits.Shining;
using LBoL.Presentation;
using LBoL.Presentation.I10N;
using LBoL.Presentation.UI;
using LBoL.Presentation.UI.Panels;
using LBoLEntitySideloader.Entities;
using LBoLEntitySideloader.Entities.Patches;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using UnityEngine;

namespace LBoLEntitySideloader
{

    [BepInPlugin(LBoLEntitySideloader.PluginInfo.GUID, LBoLEntitySideloader.PluginInfo.description, LBoLEntitySideloader.PluginInfo.version)]
    [BepInDependency("com.bepis.bepinex.scriptengine", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInProcess("LBoL.exe")]
    public class BepinexPlugin : BaseUnityPlugin
    {

        internal static BepInEx.Logging.ManualLogSource log;

        private static Harmony harmony = LBoLEntitySideloader.PluginInfo.harmony;

        
        public static ConfigEntry<bool> devModeConfig;

        public static ConfigEntry<bool> devExtraLoggingConfig;


        public static ConfigEntry<KeyboardShortcut> reloadKeyConfig;

        public static ConfigEntry<KeyboardShortcut> hardReloadKeyConfig;


        public static ConfigEntry<bool> autoRestartLevelConfig;


        public static BepinexPlugin instance;
        private void Awake()
        {
            instance = this;
            log = Logger;

            // very important. Without it the entry point MonoBehaviour gets destroyed
            DontDestroyOnLoad(gameObject);
            gameObject.hideFlags = HideFlags.HideAndDontSave;

            devModeConfig = Config.Bind("DevMode", "DevMode", false, "Enables mod developer mode for extra functionality and error feedback.");

            devExtraLoggingConfig = Config.Bind("DevMode", "ExtraLogging", true, "Enables some additional error feedback when devMode is enabled.");

            reloadKeyConfig = Config.Bind("DevMode", "ReloadKey", new KeyboardShortcut(KeyCode.F6), "Reload all entities (requires scriptengine).");

            hardReloadKeyConfig = Config.Bind("DevMode", "HardReloadKey", new KeyboardShortcut(KeyCode.None), "Hard reload localization and all entities (requires scriptengine).");

            autoRestartLevelConfig = Config.Bind("DevMode", "AutoRestart", true, "Restart level after reloading all entities.");

            ImageLoader.Init();

            ImageLoader.settings.useDiskCache = false;
            ImageLoader.settings.debugLevel = DebugLevel.Error;


            harmony.PatchAll();

        }

        private void OnDestroy()
        {
            instance = null;
            if (harmony != null)
                harmony.UnpatchSelf();
        }

        void Update()
        {

            if (devModeConfig.Value && (reloadKeyConfig.Value.IsDown() || hardReloadKeyConfig.Value.IsDown())) 
            {
                if (BepInEx.Bootstrap.Chainloader.PluginInfos.TryGetValue("com.bepis.bepinex.scriptengine", out BepInEx.PluginInfo pluginInfo))

                {
                    Reload(pluginInfo, hardReloadKeyConfig.Value.IsDown());
                }
                else
                {
                    log.LogInfo($"scriptengine is required for runtime reload");
                }
            }
        }

        static SemaphoreSlim maBoi = new SemaphoreSlim(1);
        static internal int doingMidRunReload = 0;
        /// <summary>
        /// Method for reloading all registered users while the game is running. Press F3 (by default) to reload in game. For debugging and development. Requires scriptengine. 
        /// </summary>
        /// <param name="scriptEngineInfo"></param>
        public void Reload(BepInEx.PluginInfo scriptEngineInfo, bool hardReload = false)
        {

            foreach (var user in EntityManager.Instance.sideloaderUsers.userInfos.Values)
            {   
                EntityManager.Instance.UnregisterUser(user);
            }

            foreach (var user in EntityManager.Instance.secondaryUsers.userInfos.Values)
            {
                EntityManager.Instance.UnregisterUser(user);
            }

            EntityManager.Instance.sideloaderUsers.userInfos = new Dictionary<Assembly, UserInfo>();

            EntityManager.Instance.secondaryUsers.userInfos = new Dictionary<Assembly, UserInfo>();


            UniqueTracker.DestroySelf();

            // ??
            //ImageLoader.ClearCache();

            // doesn't really help
            GC.Collect();
            GC.WaitForPendingFinalizers();


            EntityManager.Instance.loadedFromDiskUsers.Do(a => EntityManager.RegisterAssembly(a));
            EntityManager.Instance.loadedFromDiskPostAction.Do(a => UniqueTracker.Instance.PostMainLoad += a);

            UniqueTracker.Instance.formationAddActions.AddRange(EnemyGroupTemplate.loadedFromDiskCustomFormations);
            UniqueTracker.Instance.populateLoadoutInfosActions.AddRange(EntityManager.Instance.loadedFromDiskCharLoadouts);

            UniqueTracker.Instance.modifyStageListFuncs.AddRange(EntityManager.Instance.loadedFromDiskmodifyStageListFuncs);
            UniqueTracker.Instance.modifyStageActions.AddRange(EntityManager.Instance.loadedFromModifyStageActions);



            ScriptEngineWrapper.ReloadPlugins(scriptEngineInfo.Instance);


            //ensures plugins are reloaded first
            StartCoroutine(DoubleDelayAction(async () =>
            {

                if (await maBoi.WaitAsync(0))
                    try
                    {
                        ConfigDataManager.Reload();

                        if (hardReload)
                        {
                            EntityManager.Instance.RegisterUsers(EntityManager.Instance.sideloaderUsers, "All primary Sideloader users registered!");
                            EntityManager.Instance.LoadAssetsForResourceHelper(EntityManager.Instance.sideloaderUsers);

                            // reloads Sideloader loc via hookpoint
                            await L10nManager.ReloadLocalization();

                            UniqueTracker.Instance.RaisePostMainLoad();

                            EntityManager.Instance.LoadAll(EntityManager.Instance.secondaryUsers, "All secondary Sideloader users registered!", "Finished loading secondary user resources", loadLoc: false);


                        }
                        else
                        {
                            EntityManager.Instance.LoadAll(EntityManager.Instance.sideloaderUsers, "All primary Sideloader users registered!", "Finished loading primary user resources");

                            UniqueTracker.Instance.RaisePostMainLoad();
                            EntityManager.Instance.LoadAll(EntityManager.Instance.secondaryUsers, "All secondary Sideloader users registered!", "Finished loading secondary user resources");

                        }


                        UniqueTracker.Instance.populateLoadoutInfosActions.Do(a => a.Invoke());

                        if (GameMaster.Instance.CurrentGameRun == null)
                        {
                            // reload jade boxes
                            UiManager.GetPanel<StartGamePanel>()._jadeBoxToggles.Clear();
                            UiManager.GetPanel<StartGamePanel>().InitialForJadeBox();
                            // formation reload moved to HookPoints.FormationsHotReload_Patch
                            EnemyGroupTemplate.ReloadFormations();
                            PlayerSpriteLoader.ReloadForMainMenu();
                        }
                        else
                        {
                            UltimateSkillTemplate.LoadAllSpecialLoc();
                        }



                        if (autoRestartLevelConfig.Value && GameMaster.Instance.CurrentGameRun != null)
                        {
                            UiManager.GetPanel<SettingPanel>()?.UI_RestartBattle();
                            doingMidRunReload = 1;
                        }


                    }
                    catch (Exception ex)
                    {

                        log.LogError(ex);
                    }
                    finally
                    {
                        maBoi.Release();
                    }

            }));


        }

        IEnumerator DoubleDelayAction(Action action)
        {
            yield return null;
            yield return null;

            action();
        }




    }
}
