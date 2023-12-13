using BepInEx;
using GunDesigner.UI;
using HarmonyLib;
using LBoLEntitySideloader;
using LBoLEntitySideloader.Resource;
using System;
using System.Collections;
using System.Reflection;
using UnityEngine;
using UnityExplorer.Core;
using UnityExplorer.Core.UI;
using UnityExplorer.Core.UI.Panels;
using UniverseLib;
using UniverseLib.Config;


namespace GunDesigner
{
    [BepInPlugin(GunDesigner.PInfo.GUID, GunDesigner.PInfo.Name, GunDesigner.PInfo.version)]
    [BepInDependency(LBoLEntitySideloader.PluginInfo.GUID, BepInDependency.DependencyFlags.HardDependency)]
    // 2do remove
    [BepInDependency(ExplorerCore.GUID, BepInDependency.DependencyFlags.HardDependency)]
    [BepInProcess("LBoL.exe")]
    public class BepinexPlugin : BaseUnityPlugin
    {

        private static readonly Harmony harmony = GunDesigner.PInfo.harmony;

        internal static BepInEx.Logging.ManualLogSource log;

        internal static TemplateSequenceTable sequenceTable = new TemplateSequenceTable();

        internal static IResourceSource embeddedSource = new EmbeddedSource(Assembly.GetExecutingAssembly());

        // add this for audio loading
        internal static DirectorySource directorySource = new DirectorySource(GunDesigner.PInfo.GUID, "");




        private void Awake()
        {
            log = Logger;

            // very important. Without this the entry point MonoBehaviour gets destroyed
            DontDestroyOnLoad(gameObject);
            gameObject.hideFlags = HideFlags.HideAndDontSave;

            harmony.PatchAll();

            Universe.Init(1f, () => {
                UIMaster.Init();
                //UIMaster.GetPanel<PiecePanel>(UIMaster.Panels.Piece).Enabled = true;
            },
            Log,
            new UniverseLibConfig() { Force_Unlock_Mouse = true });


            AccessTools.Field(typeof(Universe), "logHandler").SetValue(null, (Action<object, LogType>)Log);

            
        }



        private void OnDestroy()
        {
            if (harmony != null)
                harmony.UnpatchSelf();

            UIMaster.DestroyAll();
        }








        private static void Log(object message, LogType logType)
        {

            if (string.IsNullOrWhiteSpace(message?.ToString()))
                return;

            if (message.ToString().Trim() == "[UniverseLib]")
                return;

            string msg = message?.ToString();
            msg = string.IsNullOrEmpty(msg) ? "Empty Log message" :  msg;
            
            switch (logType)
            {
                case LogType.Assert:
                case LogType.Log:
                    log.LogInfo(msg);
                    break;

                case LogType.Warning:
                    log.LogWarning(msg);
                    break;

                case LogType.Error:
                case LogType.Exception:
                    log.LogError(msg);
                    break;
            }
        }

    }
}
