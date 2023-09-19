using HarmonyLib;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Cards;
using LBoL.Core.Units;
using LBoL.EntityLib.Stages.NormalStages;
using LBoL.Presentation.UI.Panels;
using LBoLEntitySideloader.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using UnityEngine;
using YamlDotNet.Core.Tokens;

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
                Obj0 : "BambooForest0",
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


    }
}
