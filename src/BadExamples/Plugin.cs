using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using LBoL.Base;
using LBoL.Base.Extensions;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Adventures;
using LBoL.Core.Attributes;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActionRecord;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Battle.Interactions;
using LBoL.Core.Cards;
using LBoL.Core.Dialogs;
using LBoL.Core.GapOptions;
using LBoL.Core.Helpers;
using LBoL.Core.Intentions;
using LBoL.Core.JadeBoxes;
using LBoL.Core.PlatformHandlers;
using LBoL.Core.Randoms;
using LBoL.Core.SaveData;
using LBoL.Core.Stations;
using LBoL.Core.Stats;
using LBoL.Core.StatusEffects;
using LBoL.Core.Units;
using LBoL.EntityLib.Adventures;
using LBoL.EntityLib.Adventures.Common;
using LBoL.EntityLib.Adventures.FirstPlace;
using LBoL.EntityLib.Adventures.Shared12;
using LBoL.EntityLib.Adventures.Shared23;
using LBoL.EntityLib.Adventures.Stage1;
using LBoL.EntityLib.Adventures.Stage2;
using LBoL.EntityLib.Adventures.Stage3;
using LBoL.EntityLib.Cards.Character.Cirno;
using LBoL.EntityLib.Cards.Character.Cirno.FairySupport;
using LBoL.EntityLib.Cards.Character.Koishi;
using LBoL.EntityLib.Cards.Character.Marisa;
using LBoL.EntityLib.Cards.Character.Reimu;
using LBoL.EntityLib.Cards.Character.Sakuya;
using LBoL.EntityLib.Cards.Devel;
using LBoL.EntityLib.Cards.Neutral;
using LBoL.EntityLib.Cards.Neutral.Black;
using LBoL.EntityLib.Cards.Neutral.Blue;
using LBoL.EntityLib.Cards.Neutral.Green;
using LBoL.EntityLib.Cards.Neutral.MultiColor;
using LBoL.EntityLib.Cards.Neutral.NoColor;
using LBoL.EntityLib.Cards.Neutral.Red;
using LBoL.EntityLib.Cards.Neutral.TwoColor;
using LBoL.EntityLib.Cards.Neutral.White;
using LBoL.EntityLib.Cards.Other.Adventure;
using LBoL.EntityLib.Cards.Other.Enemy;
using LBoL.EntityLib.Cards.Other.Misfortune;
using LBoL.EntityLib.Cards.Other.Tool;
using LBoL.EntityLib.Devel;
using LBoL.EntityLib.Dolls;
using LBoL.EntityLib.EnemyUnits.Character;
using LBoL.EntityLib.EnemyUnits.Character.DreamServants;
using LBoL.EntityLib.EnemyUnits.Lore;
using LBoL.EntityLib.EnemyUnits.Normal;
using LBoL.EntityLib.EnemyUnits.Normal.Bats;
using LBoL.EntityLib.EnemyUnits.Normal.Drones;
using LBoL.EntityLib.EnemyUnits.Normal.Guihuos;
using LBoL.EntityLib.EnemyUnits.Normal.Maoyus;
using LBoL.EntityLib.EnemyUnits.Normal.Ravens;
using LBoL.EntityLib.EnemyUnits.Opponent;
using LBoL.EntityLib.Exhibits;
using LBoL.EntityLib.Exhibits.Adventure;
using LBoL.EntityLib.Exhibits.Common;
using LBoL.EntityLib.Exhibits.Mythic;
using LBoL.EntityLib.Exhibits.Seija;
using LBoL.EntityLib.Exhibits.Shining;
using LBoL.EntityLib.JadeBoxes;
using LBoL.EntityLib.Mixins;
using LBoL.EntityLib.PlayerUnits;
using LBoL.EntityLib.Stages;
using LBoL.EntityLib.Stages.NormalStages;
using LBoL.EntityLib.StatusEffects.Basic;
using LBoL.EntityLib.StatusEffects.Cirno;
using LBoL.EntityLib.StatusEffects.Enemy;
using LBoL.EntityLib.StatusEffects.Enemy.SeijaItems;
using LBoL.EntityLib.StatusEffects.Marisa;
using LBoL.EntityLib.StatusEffects.Neutral;
using LBoL.EntityLib.StatusEffects.Neutral.Black;
using LBoL.EntityLib.StatusEffects.Neutral.Blue;
using LBoL.EntityLib.StatusEffects.Neutral.Green;
using LBoL.EntityLib.StatusEffects.Neutral.Red;
using LBoL.EntityLib.StatusEffects.Neutral.TwoColor;
using LBoL.EntityLib.StatusEffects.Neutral.White;
using LBoL.EntityLib.StatusEffects.Others;
using LBoL.EntityLib.StatusEffects.Reimu;
using LBoL.EntityLib.StatusEffects.Sakuya;
using LBoL.EntityLib.UltimateSkills;
using LBoL.Presentation;
using LBoL.Presentation.Animations;
using LBoL.Presentation.Bullet;
using LBoL.Presentation.Effect;
using LBoL.Presentation.I10N;
using LBoL.Presentation.UI;
using LBoL.Presentation.UI.Dialogs;
using LBoL.Presentation.UI.ExtraWidgets;
using LBoL.Presentation.UI.Panels;
using LBoL.Presentation.UI.Transitions;
using LBoL.Presentation.UI.Widgets;
using LBoL.Presentation.Units;
using LBoLEntitySideloader;
using LBoLEntitySideloader.Attributes;
using LBoLEntitySideloader.Entities;
using LBoLEntitySideloader.Resource;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using UnityEngine;
using Untitled;
using Untitled.ConfigDataBuilder;
using Untitled.ConfigDataBuilder.Base;
using Debug = UnityEngine.Debug;


namespace BadExamples
{
    [BepInPlugin(GUID, "BadExamples", version)]
    [BepInProcess("LBoL.exe")]
    public class Plugin : BaseUnityPlugin
    {
        public const string GUID = "neo.lbol.test.BadExamples";
        public const string version = "0.0.1";

        private static readonly Harmony harmony = new Harmony(GUID);

        internal static BepInEx.Logging.ManualLogSource log;

        internal static TemplateSequenceTable sequenceTable = new TemplateSequenceTable();

        internal static IResourceSource embeddedSource = new EmbeddedSource(Assembly.GetExecutingAssembly());

        private void Awake()
        {
            log = Logger;

            // very important. Without this the entry point MonoBehaviour gets destroyed
            DontDestroyOnLoad(gameObject);
            gameObject.hideFlags = HideFlags.HideAndDontSave;
            
            EntityManager.RegisterSelf();
            harmony.PatchAll();

        }

        private void OnDestroy()
        {
            if (harmony != null)
                harmony.UnpatchSelf();
        }

        public sealed class AllGoodDefinition : CardTemplate
        {
            public override IdContainer GetId()
            {
                return nameof(AllGood);
            }

            public override CardImages LoadCardImages()
            {
                return null;
            }

            public override LocalizationOption LoadText()
            {
                return null;
            }

            public override CardConfig MakeConfig()
            {
                var cardConfig = DefaultConfig();
                cardConfig.Index = sequenceTable.Next(typeof(CardConfig));
                cardConfig.Id = "";
                cardConfig.Type = CardType.Status;
                return cardConfig;
            }


            [EntityLogic(typeof(AllGoodDefinition))]
            public sealed class AllGood : Card
            {
                public override IEnumerable<BattleAction> OnDraw()
                {
                    log.LogInfo("All good");
                    return base.OnDraw();   
                }
            }
        }


        public sealed class NoEntityLogicDefinition : CardTemplate
        {
            public override IdContainer GetId()
            {
                return "NoEntityLogic";
            }

            public override CardImages LoadCardImages()
            {
                return null;
            }

            public override LocalizationOption LoadText()
            {
                return null;
            }

            public override CardConfig MakeConfig()
            {
                var cardConfig = DefaultConfig();
                cardConfig.Index = sequenceTable.Next(typeof(CardConfig));
                cardConfig.Id = "";
                cardConfig.Type = CardType.Status;
                return cardConfig;
            }


        }

        public sealed class WrongTypeInEntityLogicAttributeDefinition : CardTemplate
        {
            public override IdContainer GetId()
            {
                return nameof(WrongTypeInEntityLogicAttribute);
            }

            public override CardImages LoadCardImages()
            {
                return null;
            }

            public override LocalizationOption LoadText()
            {
                return null;
            }

            public override CardConfig MakeConfig()
            {
                var cardConfig = DefaultConfig();
                cardConfig.Index = sequenceTable.Next(typeof(CardConfig));
                cardConfig.Id = "";
                cardConfig.Type = CardType.Status;
                return cardConfig;
            }


            [EntityLogic(typeof(Card))]
            public sealed class WrongTypeInEntityLogicAttribute : Card
            {
                public override IEnumerable<BattleAction> OnDraw()
                {
                    log.LogInfo("WrongTypeInEntityLogicAttribute");
                    return base.OnDraw();
                }
            }
        }




        // mismatched id and type name
        public sealed class NutsDefinition : CardTemplate
        {
            public override IdContainer GetId()
            {
                return "Nuts";
            }

            public override CardImages LoadCardImages()
            {
                return null;
            }

            public override LocalizationOption LoadText()
            {
                return null;
            }


            public override CardConfig MakeConfig()
            {
                var cardConfig = DefaultConfig();
                cardConfig.Index = sequenceTable.Next(typeof(CardConfig));
                cardConfig.Id = "";
                cardConfig.Type = CardType.Misfortune;
                return cardConfig;
            }



            [EntityLogic(typeof(NutsDefinition))]
            public sealed class DeeeeeeezNuts : Card
            {
                public override IEnumerable<BattleAction> OnDraw()
                {
                    log.LogInfo("Nuts");
                    return base.OnDraw();
                }
            }
        }


        // duplicate id and entity logic type name
        // no automatic unique id enforcing inside a the same plugin
        public sealed class DeezDefinition : CardTemplate
        {
            public override IdContainer GetId()
            {
                return nameof(Deeznuts);
            }

            public override CardImages LoadCardImages()
            {
                return null;
            }

            public override LocalizationOption LoadText()
            {
                return null;
            }   

            public override CardConfig MakeConfig()
            {
                var cardConfig = DefaultConfig();
                cardConfig.Index = 42020;
                cardConfig.Id = GetId();
                cardConfig.Type = CardType.Misfortune;
                return cardConfig;
            }



            [EntityLogic(typeof(DeezDefinition))]
            public sealed class Deeznuts : Card
            {
                public override IEnumerable<BattleAction> OnDraw()
                {
                    log.LogInfo("Deez");
                    return base.OnDraw();
                }
            }
        }


        // no vanilla card with id Deeznuts
        // can't overwrite modded stuff
        [OverwriteVanilla]
        public sealed class DeeznutsOverwriteDefinition : CardTemplate
        {
            public override IdContainer GetId()
            {
                return nameof(Deeznuts);
            }

            public override CardImages LoadCardImages()
            {
                return null;
            }

            public override LocalizationOption LoadText()
            {
                return null;
            }

            public override CardConfig MakeConfig()
            {
                var cardConfig = DefaultConfig();
                cardConfig.Index = sequenceTable.Next(typeof(CardConfig));
                cardConfig.Id = "";
                cardConfig.Type = CardType.Status;
                return cardConfig;
            }


            [EntityLogic(typeof(DeeznutsOverwriteDefinition))]
            public sealed class Deeznuts : Card
            {
                public override IEnumerable<BattleAction> OnDraw()
                {
                    log.LogInfo("Deeznuts");
                    return base.OnDraw();
                }
            }
        }



        [OverwriteVanilla]
        public sealed class ModifyOpenUniverseDefinition : CardTemplate
        {
            public override IdContainer GetId()
            {
                return nameof(OpenUniverse);
            }

            public override CardImages LoadCardImages()
            {
                return null;
            }

            [DontOverwrite]
            public override LocalizationOption LoadText()
            {
                return null;
            }

            public override CardConfig MakeConfig()
            {
                
                return CardConfig.FromId(UniqueId);
            }


            [EntityLogic(typeof(ModifyOpenUniverseDefinition))]
            public sealed class OpenUniverse : Card
            {
                public override IEnumerable<BattleAction> OnDraw()
                {
                    log.LogInfo("Open1");
                    return base.OnDraw();
                }
            }
        }


        [OverwriteVanilla]
        public sealed class OverwriteTextOnlyDefinition : CardTemplate
        {
            public override IdContainer GetId()
            {
                return "OpenUniverse";
            }

            [DontOverwrite]
            public override CardImages LoadCardImages()
            {
                return null;
            }


            public override LocalizationOption LoadText()
            {
                var locFiles = new LocalizationFiles(embeddedSource);
                return null;
            }

            [DontOverwrite]
            public override CardConfig MakeConfig()
            {

                return null;
            }


        }


        [OverwriteVanilla]
        public sealed class DuplicateOverwriteDefinition : CardTemplate
        {
            public override IdContainer GetId()
            {
                return nameof(OpenUniverse);
            }

            public override CardImages LoadCardImages()
            {
                return null;
            }

            public override LocalizationOption LoadText()
            {
                return null;
            }

            public override CardConfig MakeConfig()
            {

                return CardConfig.FromId(UniqueId);
            }



        }

        [EntityLogic(typeof(DuplicateOverwriteDefinition))]
        public sealed class OpenUniverse : Card
        {
            public override IEnumerable<BattleAction> OnDraw()
            {
                log.LogInfo("Open3");
                return base.OnDraw();
            }
        }


    }
}
