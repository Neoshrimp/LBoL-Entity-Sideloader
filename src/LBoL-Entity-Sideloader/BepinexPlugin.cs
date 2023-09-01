using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using LBoL.ConfigData;
using LBoL.Presentation;
using LBoL.Presentation.I10N;
using LBoL.Presentation.UI;
using LBoL.Presentation.UI.Panels;
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

            reloadKeyConfig = Config.Bind("DevMode", "ReloadKey", new KeyboardShortcut(KeyCode.F3), "Reload all entities (requires scriptengine).");

            hardReloadKeyConfig = Config.Bind("DevMode", "HardReloadKey", new KeyboardShortcut(KeyCode.F7), "Hard reload localization and all entities (requires scriptengine).");

            autoRestartLevelConfig = Config.Bind("DevMode", "AutoRestart", true, "Restart level after reloading all entities.");


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
                    log.LogInfo($"BePinEx scriptengine is needed to use runtime reload");
                }
            }
        }

        static SemaphoreSlim maBoi = new SemaphoreSlim(1);

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



            ScriptEngineWrapper.ReloadPlugins(scriptEngineInfo.Instance);

            //ensures plugins are reloaded first
            StartCoroutine(DoubleDelayAction(async () =>
            {

                if (await maBoi.WaitAsync(0))
                    try
                    {

                        // doesn't really help
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                        GC.Collect();
                        

                        EntityManager.Instance.loadedFromDisk.Do(a => EntityManager.RegisterAssembly(a));
                        EntityManager.Instance.loadedFromDiskPostAction.Do(a => UniqueTracker.Instance.PostMainLoad += a);

                        ConfigDataManager.Reload();

                        if (hardReload)
                        {
                            EntityManager.Instance.RegisterUsers(EntityManager.Instance.sideloaderUsers);
                            EntityManager.Instance.LoadAssetsForResourceHelper(EntityManager.Instance.sideloaderUsers);

                            // reloads Sideloader loc via hookpoint
                            await L10nManager.ReloadLocalization();

                            UniqueTracker.Instance.RaisePostMainLoad();

                            EntityManager.Instance.LoadAll(EntityManager.Instance.secondaryUsers);


                        }
                        else
                        {
                            EntityManager.Instance.LoadAll(EntityManager.Instance.sideloaderUsers);

                            UniqueTracker.Instance.RaisePostMainLoad();
                            EntityManager.Instance.LoadAll(EntityManager.Instance.secondaryUsers);

                        }



                        if (autoRestartLevelConfig.Value && GameMaster.Instance.CurrentGameRun != null)
                        {
                            UiManager.GetPanel<SettingPanel>()?.UI_RestartBattle();
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
