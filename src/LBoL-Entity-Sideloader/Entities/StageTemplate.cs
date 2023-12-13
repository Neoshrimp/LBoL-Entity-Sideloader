using HarmonyLib;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Cards;
using LBoL.Core.Units;
using LBoL.EntityLib.Stages.NormalStages;
using LBoL.Presentation;
using LBoL.Presentation.UI.Panels;
using LBoL.Presentation.Units;
using LBoLEntitySideloader.ReflectionHelpers;
using LBoLEntitySideloader.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.WebSockets;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using UnityEngine;
using YamlDotNet.Core.Tokens;
using Environment = LBoL.Presentation.Environment;

namespace LBoLEntitySideloader.Entities
{
    public abstract class StageTemplate : EntityDefinition,
        IConfigProvider<StageConfig>,
        IGameEntityProvider<Stage>
    {
        public override Type ConfigType() => typeof(StageConfig);
        public override Type EntityType() => typeof(Stage);
        public override Type TemplateType() => typeof(StageTemplate);

        /// <summary>
        /// Stage RandomPools should be added or removed from rather than completely replaced
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="stageMod"></param>
        /// <param name="callingAssembly"></param>
        public static void ModifyStage(string Id, Func<Stage, Stage> stageMod, Assembly callingAssembly = null)
        {
            if (callingAssembly == null)
                callingAssembly = Assembly.GetCallingAssembly();

            var stageModAction = new UniqueTracker.StageModAction() { Id = Id, mod = stageMod };

            if (callingAssembly.IsLoadedFromDisk())
                EntityManager.Instance.loadedFromModifyStageActions.Add(stageModAction);

            UniqueTracker.Instance.modifyStageActions.Add(stageModAction);

        }

        public static void ModifyStageList(Func<List<Stage>, List<Stage>> listMod, Assembly callingAssembly = null)
        {
            if (callingAssembly == null)
                callingAssembly = Assembly.GetCallingAssembly();

            if (callingAssembly.IsLoadedFromDisk())
                EntityManager.Instance.loadedFromDiskmodifyStageListFuncs.Add(listMod);

            UniqueTracker.Instance.modifyStageListFuncs.Add(listMod);
        }


        internal static HashSet<string> customEnvs = new HashSet<string>();
        internal static HashSet<Action> loadedFromDiskEnvironments = new HashSet<Action>();
        internal static HashSet<string> currentEnvs = new HashSet<string>();

        /// <summary>
        /// Gets reference to environment object created via AddEvironmentGameobject.   
        /// Save to use in entity logic methods to perform actions.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="userAssembly"></param>
        /// <returns></returns>
        public static GameObject TryGetEnvObject(string name, Assembly userAssembly = null)
        {
            if (userAssembly == null)
                userAssembly = Assembly.GetCallingAssembly();

            GameObject go = null;

            if (UniqueTracker.Instance.createdEnvObjectCache.TryGetValue(userAssembly, out var goDic))
                goDic.TryGetValue(name, out go);
            
            return go;
        }

        /// <summary>
        /// Creates function which creates simple background object.
        /// Game uses 3230x1822 sprite with ppu at 100 but generally background size is pretty lenient
        /// </summary>
        /// <param name="name"></param>
        /// <param name="background"></param>
        /// <returns></returns>
        public static Func<GameObject> CreateSimpleEnvObject(string name, Sprite background)
        {
            return () =>
            {
                var envGo = new GameObject(name);
                var envO = envGo.AddComponent<EnvironmentObject>();
                envO.name = envGo.name;
                if (background != null)
                {
                    var sr = envGo.AddComponent<SpriteRenderer>();
                    sr.sprite = background;
                }

                return envGo;
            };
        }

        /// <summary>
        /// Defers env object creation after Environment.Awake is called.
        /// Should be used in BepinexPlugin.Awake()
        /// After setting root object position will be reset to 0, 0, 0. adjustAfterSettingRoot Func can be used to change that.
        /// </summary>
        /// <param name="getEvnGo"></param>
        /// <param name="managedByEnvironment">if true the env object can be used in StageConfig</param>
        /// <param name="adjustAfterSettingRoot"></param>
        /// <param name="overwriteName"></param>
        /// <param name="callingAssembly"></param>
        public static void AddEvironmentGameobject(Func<GameObject> getEvnGo, bool managedByEnvironment, Func<GameObject, GameObject> adjustAfterSettingRoot = null, string overwriteName = null, Assembly callingAssembly = null)
        {
            if (callingAssembly == null)
                callingAssembly = Assembly.GetCallingAssembly();

            Action action = () => {
                if (Environment.Instance != null)
                {
                    var envGo = getEvnGo();

                    if (!string.IsNullOrEmpty(overwriteName))
                        envGo.name = overwriteName;

                    envGo.transform.SetParent(Environment.Instance.stageRoot);
                    envGo.transform.position = Environment.Instance.stageRoot.position;
                    envGo.SetActive(false);

                    var newEvnGo = adjustAfterSettingRoot?.Invoke(envGo);

                    if (newEvnGo != null)
                        envGo = newEvnGo;

                    customEnvs.Add(envGo.name);

                    if (managedByEnvironment)
                    {
                        if (!envGo.TryGetComponent<EnvironmentObject>(out var envO))
                        {
                            envO = envGo.AddComponent<EnvironmentObject>();
                            envO.name = envGo.name;
                        }
                        if (currentEnvs.Count == 0)
                        {
                            Environment.Instance.simpleTemplates.Do(eo => currentEnvs.Add(eo.name));
                            Environment.Instance.templates.Do(eo => currentEnvs.Add(eo.name));
                        }
                        if (!currentEnvs.Contains(envO.name))
                        {
                            Environment.Instance.simpleTemplates.Add(envO);
                            Environment.Instance.templates.Add(envO);
                            currentEnvs.Add(envO.name);
                        }
                        else
                        {
                            Log.log.LogWarning($"EnvironmentObject with name {envO.name} is already managed by Environment and will not be added.");
                        }
                    }


                    UniqueTracker.Instance.createdEnvObjectCache.TryAdd(callingAssembly, new Dictionary<string, GameObject>());

                    if (!UniqueTracker.Instance.createdEnvObjectCache[callingAssembly].TryAdd(envGo.name, envGo))
                    {
                        Log.log.LogError($"Caching environmental object {envGo.name} failed. GameObject reference will be inaccessible");
                    }
                }
            };


            if (callingAssembly.IsLoadedFromDisk())
                loadedFromDiskEnvironments.Add(action);

            UniqueTracker.Instance.environmentsAddActions.Add(action);
        }


        static internal void ReloadEnvs()
        {
            var envsToDestroy = FindEnvsToUnload();
            UnloadCustomEnvs(envsToDestroy);
            // loaded from disk envs added elsewhere. for some reason
            LoadCustomEnvironments();
        }

        static internal void LoadCustomEnvironments()
        {
            UniqueTracker.Instance.environmentsAddActions.Do(a => a.Invoke());
        }


        static internal void UnloadCustomEnvs(List<GameObject> goToDestroy)
        {

            var environment = Environment.Instance;
            var envOs = new HashSet<EnvironmentObject>();

            foreach (var go in goToDestroy)
            {
                if (go.TryGetComponent<EnvironmentObject>(out var envO))
                {
                    envOs.Add(envO);
                    currentEnvs.Remove(envO.name);
                }
                GameObject.Destroy(go);
            }
            if (environment != null)
            {
                environment.simpleTemplates = environment.simpleTemplates.Where(eo => !envOs.Contains(eo)).ToList();
                environment.templates = environment.templates.Where(eo => !envOs.Contains(eo)).ToList();
            }
        }

        static internal List<GameObject> FindEnvsToUnload()
        {
            var envsToDestroy = new List<GameObject>();
            var root = Environment.Instance?.stageRoot;
            if (root == null)
                return envsToDestroy;
            foreach (var trans in root)
            {
                var go = (trans as Transform).gameObject;
                if (customEnvs.Contains(go.name))
                {
                    envsToDestroy.Add(go);
                }
            }
            customEnvs.Clear();

            return envsToDestroy;
        }



        public static class VanillaEnvironments
        {
            public const string BambooForest0 = "BambooForest0";
            public const string BambooForest1 = "BambooForest1";
            public const string BambooForest2 = "BambooForest2";
            public const string BambooForest3 = "BambooForest3";
            public const string XuanwuRavine1 = "XuanwuRavine1";
            public const string XuanwuRavine3 = "XuanwuRavine3";
            public const string WindGodLake1 = "WindGodLake1";
            public const string WindGodLake2 = "WindGodLake2";
        }

        /// <summary>
        /// Id : ,
        /// Obj0 : background environment,
        /// Obj1 : and so on,
        /// Level1 : on which (greater or equal)stage level Obj0 background should be replaced with Obj1 background?,
        /// Obj2 : and so on,
        /// Level2 : and so on,
        /// Obj3 : and so on,
        /// Level3 : and so on,
        /// Obj4 : and so on,
        /// Level4 and so on: 
        /// </summary>
        /// <returns></returns>
        public StageConfig DefaultConfig()
        {
            var config = new StageConfig(
                Id : "",
                Obj0 : VanillaEnvironments.BambooForest0,
                Obj1 : "",
                Level1 : 1,
                Obj2 : "",
                Level2 : 0,
                Obj3 : "",
                Level3 : 0,
                Obj4 : VanillaEnvironments.WindGodLake2,
                Level4 : 0
                );
            return config;
        }
        public abstract StageConfig MakeConfig();



        [HarmonyPatch(typeof(StartGamePanel), "OnShowing")]
        class AddStage_Patch
        {
            static Stage[] ModStageArray(Stage[] stages)
            {
                var list = stages.ToList();
                foreach (var f in UniqueTracker.Instance.modifyStageListFuncs)
                {
                    list = f.Invoke(list);
                }
                var id2Stage = new Dictionary<string, Stage>();
                var id2Index = new Dictionary<string, int>();
                int i = 0;
                foreach (var s in list)
                {
                    id2Stage.Add(s.Id, s);
                    id2Index.Add(s.Id, i);
                    i++;
                }
                list.ToDictionary(s => s.Id);

                foreach (var sm in UniqueTracker.Instance.modifyStageActions)
                {
                    if (!id2Stage.ContainsKey(sm.Id))
                    {
                        Log.LogDev()?.LogWarning($"Trying to modify stage with Id:{sm.Id} but it wasn't loaded. It either doesn't exist or was removed from stage list");
                        continue;
                    }
                    list[id2Index[sm.Id]] = sm.mod.Invoke(id2Stage[sm.Id]);
                }

                return list.ToArray();
            }

            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
            {
                return new CodeMatcher(instructions, generator)
                    .MatchForward(false, new CodeMatch(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(StartGameData), nameof(StartGameData.StagesCreateFunc))))
                    .Advance(2)
                    .Insert(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(AddStage_Patch), nameof(AddStage_Patch.ModStageArray))))
                    .InstructionEnumeration();
            }

        }



        [HarmonyPatch]
        class EnableCustomBg_Patch
        {

            static IEnumerable<MethodBase> TargetMethods()
            {
                yield return ExtraAccess.InnerMoveNext(typeof(Environment), nameof(Environment.LoadEnvironment));
            }

            static void Postfix()
            {
                Environment.CurrentEnvironment.gameObject.SetActive(true);
            }
        }


    }
}
