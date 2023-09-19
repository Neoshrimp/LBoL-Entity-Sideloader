using HarmonyLib;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Randoms;
using LBoL.EntityLib.Exhibits.Shining;
using LBoL.EntityLib.Stages.NormalStages;
using LBoL.Presentation.UI.Panels;
using LBoLEntitySideloader;
using LBoLEntitySideloader.Attributes;
using LBoLEntitySideloader.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using static Random_Examples.BepinexPlugin;

namespace Random_Examples
{

    public sealed class NewStageExDef : StageTemplate
    {
        public override IdContainer GetId() => nameof(NewStageEx);


        public override StageConfig MakeConfig()
        {
            return DefaultConfig();
        }

        [EntityLogic(typeof(NewStageExDef))]
        public sealed class NewStageEx : NormalStageBase
        {
            public NewStageEx()
            {
                base.Level = 1;
                base.CardUpgradedChance = 0f;
                base.IsSelectingBoss = true;
                base.EnemyPoolAct1 = new UniqueRandomPool<string>(true)
            {
                { "Sanyue", 1f },
                { "Aya", 1f },
                { "Rin", 1f }
            };
                base.EnemyPoolAct2 = new UniqueRandomPool<string>(true)
            {
                { "Sanyue", 1f },
                { "Aya", 1f },
                { "Rin", 1f }
            };
                base.EnemyPoolAct3 = new UniqueRandomPool<string>(true)
            {
                { "Sanyue", 1f },
                { "Aya", 1f },
                { "Rin", 1f }
            };
                base.EliteEnemyPool = new UniqueRandomPool<string>(true)
            {
                { "Sanyue", 1f },
                { "Aya", 1f },
                { "Rin", 1f }
            };
            }
        }
    }

    public sealed class AnotherNewStageExDef : StageTemplate
    {
        public override IdContainer GetId() => nameof(AnotherNewStage);


        public override StageConfig MakeConfig()
        {
            return DefaultConfig();
        }

        [EntityLogic(typeof(AnotherNewStageExDef))]
        public sealed class AnotherNewStage : NormalStageBase
        {
            public AnotherNewStage()
            {
                base.Level = 1;
                base.CardUpgradedChance = 0f;
                base.IsSelectingBoss = true;
                base.EnemyPoolAct1 = new UniqueRandomPool<string>(true)
            {
                { "11", 1f },
                { "11", 1f },
                { "11", 1f }
            };
                base.EnemyPoolAct2 = new UniqueRandomPool<string>(true)
            {
                { "11", 1f },
                { "11", 1f },
                { "11", 1f }
            };
                base.EnemyPoolAct3 = new UniqueRandomPool<string>(true)
            {
                { "11", 1f },
                { "11", 1f },
                { "11", 1f }
            };
                base.EliteEnemyPool = new UniqueRandomPool<string>(true)
            {
                { "11", 1f },
                { "11", 1f },
                { "11", 1f }
            };
            }
        }
    }





    public class StageExamples
    {

        static public void AddStages()
        {
            StageTemplate.ModifyStageList((List<Stage> list) =>
            {
                list.Insert(1, Library.CreateStage(new NewStageExDef().UniqueId));
                // more stages can be inserted
                return list;
            });

            // for test. AnotherNewStageExDef should be the first stage of the run
            StageTemplate.ModifyStageList((List<Stage> list) =>
            {
                list.Insert(1, Library.CreateStage(new AnotherNewStageExDef().UniqueId));
                return list;
            });

            StageTemplate.ModifyStage(nameof(BambooForest), (Stage stage) =>
            {
                // can be new 
                stage.EnemyPoolAct1.Add(new BallsAndGirlDef().UniqueId, 1.5f);
                stage.EnemyPoolAct1.Add(new ThreeCrowsGroupDef().UniqueId, 1.5f);

                return stage;
            });

        }

    }


}
