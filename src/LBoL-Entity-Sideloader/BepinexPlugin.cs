using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using LBoL.ConfigData;
using LBoL.Presentation;
using LBoL.Presentation.UI;
using LBoL.Presentation.UI.Panels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace LBoLEntitySideloader
{

    [BepInPlugin(PluginInfo.GUID, PluginInfo.description, PluginInfo.version)]
    [BepInDependency("com.bepis.bepinex.scriptengine", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInProcess("LBoL.exe")]
    public class BepinexPlugin : BaseUnityPlugin
    {

        internal static BepInEx.Logging.ManualLogSource log;

        private static Harmony harmony = PluginInfo.harmony;

        
        public static ConfigEntry<bool> devModeConfig;

        public static ConfigEntry<bool> devExtraLoggingConfig;


        public static ConfigEntry<KeyboardShortcut> reloadKeyConfig;

        public static ConfigEntry<bool> autoRestartLevelConfig;


        public static BepinexPlugin Instance;
        private void Awake()
        {
            Instance = this;

            log = Logger;

            // very important. Without it the entry point MonoBehaviour gets destroyed
            DontDestroyOnLoad(gameObject);
            gameObject.hideFlags = HideFlags.HideAndDontSave;

            devModeConfig = Config.Bind("DevMode", "DevMode", false, "Enables mod developer mode for extra functionality and error feedback.");

            devExtraLoggingConfig = Config.Bind("DevMode", "ExtraLogging", true, "Enables some additional error feedback when devMode is enabled.");

            reloadKeyConfig = Config.Bind("DevMode", "ReloadKey", new KeyboardShortcut(KeyCode.F3), "Hard reload all entities (requires scriptengine).");

            autoRestartLevelConfig = Config.Bind("DevMode", "AutoRestart", true, "Restart level after reloading all entities.");


            harmony.PatchAll();

        }

        private void OnDestroy()
        {
            if (harmony != null)
                harmony.UnpatchSelf();
        }

        void Update()
        {

            if (devModeConfig.Value && reloadKeyConfig.Value.IsDown()) 
            {
                if (BepInEx.Bootstrap.Chainloader.PluginInfos.TryGetValue("com.bepis.bepinex.scriptengine", out BepInEx.PluginInfo pluginInfo))

                {
                    HardReload(pluginInfo);
                }
                else
                {
                    log.LogInfo($"BePinEx scriptengine is needed to use runtime reload");
                }
            }
        }

        /// <summary>
        /// Method for reloading all registered users while the game is running. Press F3 (by default) to reload in game. For debugging and developing. Requires scriptengine. 
        /// </summary>
        /// <param name="scriptEngineInfo"></param>
        public void HardReload(BepInEx.PluginInfo scriptEngineInfo)
        {

            foreach (var user in EntityManager.Instance.sideloaderUsers.userInfos.Values)
            {
                EntityManager.Instance.UnregisterUser(user);
            }

            EntityManager.Instance.sideloaderUsers.userInfos = new Dictionary<Assembly, UserInfo>();


            ScriptEngineWrapper.ReloadPlugins(scriptEngineInfo.Instance);

            //ensures plugins are reloaded first
            StartCoroutine(DoubleDelayAction(() =>
            {
                UniqueTracker.DestroySelf();

                EntityManager.Instance.loadedFromDisk.Do(a => EntityManager.RegisterAssembly(a));

                ConfigDataManager.Reload();

                EntityManager.Instance.LoadAll();


                if (autoRestartLevelConfig.Value && GameMaster.Instance.CurrentGameRun != null)
                {
                    UiManager.GetPanel<SettingPanel>()?.UI_RestartBattle();
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
