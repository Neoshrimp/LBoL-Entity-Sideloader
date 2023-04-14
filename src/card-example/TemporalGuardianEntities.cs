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
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using UnityEngine;
using Untitled;
using Untitled.ConfigDataBuilder;
using Untitled.ConfigDataBuilder.Base;
using Debug = UnityEngine.Debug;
using LBoLEntitySideloader;
using LBoLEntitySideloader.Attributes;
using LBoLEntitySideloader.Entities;
using LBoLEntitySideloader.Resources;

namespace CardExample
{
    public  class TemporalGuardianEntities
    {

        private static BepInEx.Logging.ManualLogSource log = Plugin.log;

        internal static TemplateSequenceTable sequenceTable = Plugin.sequenceTable;


        public sealed class TemporalGuardianDefinition : CardTemplate
        {

            public override IdContainer GetId() 
            {

                return nameof(TemporalGuardian);
            }

            public override CardImages Load()
            {
                return new CardImages(ResourceLoader.LoadTexture(GetId()+".png", Plugin.directorySource));
            }

            public override CardConfig MakeConfig()
            {

                var cardConfig = new CardConfig(
                               //2do reloading problem
                               Index: sequenceTable.Next(typeof(CardConfig)),
                               Id: GetId(),
                               Order: 10,
                               AutoPerform: true,
                               Perform: new string[0][],
                               GunName: "Simple1",
                               GunNameBurst: "Simple1",
                               DebugLevel: 0,
                               // for debug
                               Revealable: false,
                               IsPooled: true,
                               IsUpgradable: true,
                               Rarity: Rarity.Uncommon,
                               Type: CardType.Ability,
                               TargetType: TargetType.Self,
                               Colors: new List<ManaColor>() { ManaColor.Blue, ManaColor.White },
                               IsXCost: false,
                               Cost: new ManaGroup() { Any = 1, Blue = 1, White = 1 },
                               UpgradedCost: new ManaGroup() { Any = 1 },
                               MoneyCost: null,
                               Damage: null,
                               UpgradedDamage: null,
                               Block: null,
                               UpgradedBlock: null,
                               Shield: null,
                               UpgradedShield: null,
                               Value1: 3,
                               UpgradedValue1: null,
                               Value2: null,
                               UpgradedValue2: null,
                               Mana: null,
                               UpgradedMana: null,
                               Scry: null,
                               UpgradedScry: null,
                               ToolPlayableTimes: null,
                               Keywords: Keyword.None,
                               UpgradedKeywords: Keyword.None,
                               EmptyDescription: false,
                               // what does it do?
                               RelativeKeyword: Keyword.None,
                               UpgradedRelativeKeyword: Keyword.None,
                               RelativeEffects: new List<string>() { "TimeAuraSe" },
                               UpgradedRelativeEffects: new List<string>() { "TimeAuraSe" },
                               RelativeCards: new List<string>(),
                               UpgradedRelativeCards: new List<string>(),
                               Owner: "Sakuya",
                               Unfinished: false,
                               Illustrator: null,
                               SubIllustrator: new List<string>() { "alt" }
                    );
                return cardConfig;
            }



            [EntityLogic(typeof(TemporalGuardianDefinition))]
            public sealed class TemporalGuardian : Card
            {
                protected override IEnumerable<BattleAction> Actions(UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
                {
                    yield return base.BuffAction<TemporialGuardianSeDefinition.TemporalGuardianSe>(base.Value1, 0, 0, 0, 0.3f);
                    yield break;
                }
            }


        }


        public sealed class TemporialGuardianSeDefinition : StatusEffectTemplate
        {

            public override IdContainer GetId()
            {
                return nameof(TemporalGuardianSe);
            }

            public override StatusEffectConfig MakeConfig()
            {

                var se = new StatusEffectConfig(
                                Id: GetId(),
                                Order: 10,
                                Type: StatusEffectType.Positive,
                                IsVerbose: false,
                                IsStackable: true,
                                StackActionTriggerLevel: null,
                                HasLevel: true,
                                LevelStackType: StackType.Add,
                                HasDuration: false,
                                DurationStackType: StackType.Add,
                                DurationDecreaseTiming: DurationDecreaseTiming.Custom,
                                HasCount: false,
                                CountStackType: StackType.Keep,
                                LimitStackType: StackType.Keep,
                                ShowPlusByLimit: false,
                                Keywords: Keyword.None,
                                RelativeEffects: new List<string>() { "TimeAuraSe" },
                                VFX : "BuffBlue",
                                VFXloop: "Default",
                                SFX: "Default"
                            );

                return se;
            }



            [HarmonyPatch(typeof(StatusEffect), nameof(StatusEffect.NotifyChanged))]
            class StatusEffect_Patch
            {

                static void Postfix(StatusEffect __instance)
                {
                    if (__instance is TimeAuraSe ta && __instance.Battle != null &&  __instance.Owner == __instance.Battle.Player &&  __instance.Battle.Player.HasStatusEffect<TemporalGuardianSe>())
                    {

                        
                        new CastBlockShieldAction(__instance.Battle.Player, new ShieldInfo(ta.Level * __instance.Battle.Player.GetStatusEffect<TemporalGuardianSe>().Level, BlockShieldType.Direct), false);
                    }
                }
            }



            //[HarmonyPatch(typeof(ActionResolver), nameof(ActionResolver.React))]
            class ActionResolver_Patch
            {
                static void Prefix(ActionResolver __instance)
                {
                    if (__instance._reactors == null)
                    {
                        __instance._reactors = new List<Reactor>();
                    }
                }

            }






            [EntityLogic(typeof(TemporialGuardianSeDefinition))]
            public sealed class TemporalGuardianSe : StatusEffect
            { 

                protected override void OnAdded(Unit unit)
                {

                    ReactOwnerEvent(Battle.Player.StatusEffectAdding, new EventSequencedReactor<StatusEffectApplyEventArgs>(TimePulseAdding));

                    ReactOwnerEvent(Battle.Player.StatusEffectAdded, new EventSequencedReactor<StatusEffectApplyEventArgs>(TimePulseAdded));

                    ReactOwnerEvent(Battle.Player.StatusEffectChanged, new EventSequencedReactor<StatusEffectEventArgs>(TimePulseChange), GameEventPriority.Highest);

                    ReactOwnerEvent(Battle.Player.StatusEffectChanged, new EventSequencedReactor<StatusEffectEventArgs>(ChargeChange), GameEventPriority.Lowest);

                    HandleOwnerEvent(Battle.Player.StatusEffectAdded, new GameEventHandler<StatusEffectApplyEventArgs>(TimeAuraHandler));

                    ReactOwnerEvent(Battle.Player.StatusEffectRemoved, new EventSequencedReactor<StatusEffectEventArgs>(TimePulseRemoved));

                    ReactOwnerEvent(Battle.Player.StatusEffectRemoving, new EventSequencedReactor<StatusEffectEventArgs>(TimePulseRemoving));
                }


                private IEnumerable<BattleAction> ChargeChange(StatusEffectEventArgs args)
                {
                    if (args.Effect is Charging c)
                    {
                        NotifyActivating();
                        log.LogDebug($"changed: level: {c.Level}, trigger level: {c.TriggerLevel}, {args.Unit}");
                    }
                    yield break;
                }

                private IEnumerable<BattleAction> TimePulseChange(StatusEffectEventArgs args)
                {
                    if (args.Effect is TimeAuraSe ta)
                    {
                        NotifyActivating();
                        log.LogDebug($"changed: level: {ta.Level}, trigger level: {ta.TriggerLevel}");
                    }
                    yield break;
                }


                private void TimeAuraHandler(StatusEffectApplyEventArgs args)
                {
                    if (args.Effect is TimeAuraSe ta)
                    {
                        NotifyActivating();
                        log.LogDebug($"deeznuts");
                        args.Effect.Level += 10;
                    }
                }

                private IEnumerable<BattleAction> TimePulseAdding(StatusEffectApplyEventArgs args)
                {
                    if (args.Effect is TimeAuraSe ta)
                    {
                        NotifyActivating();
                        log.LogDebug($"addING: level: {ta.Level}, trigger level: {ta.TriggerLevel}, args Level:{args.Level}");
                    }
                    yield break;
                }

                private IEnumerable<BattleAction> TimePulseAdded(StatusEffectApplyEventArgs args)
                {
                    if (args.Effect is TimeAuraSe ta)
                    {
                        NotifyActivating();
                        log.LogDebug($"addED: level: {ta.Level}, trigger level: {ta.TriggerLevel}, args Level:{args.Level}");
                    }
                    yield break;
                }



                private IEnumerable<BattleAction> TimePulseRemoved(StatusEffectEventArgs args)
                {
                    if (args.Effect is TimeAuraSe ta)
                    {
                        NotifyActivating();
                        var current_lvl = ta.Level;
                        yield return new CastBlockShieldAction(Battle.Player, new ShieldInfo(current_lvl * Level, BlockShieldType.Direct), false);
                        log.LogDebug($"removED: level: {ta.Level}, trigger level: {ta.TriggerLevel}");
                    }
                    yield break;
                }

                private IEnumerable<BattleAction> TimePulseRemoving(StatusEffectEventArgs args)
                {
                    if (args.Effect is TimeAuraSe ta)
                    {
                        NotifyActivating();
                        log.LogDebug($"removING: level: {ta.Level}, trigger level: {ta.TriggerLevel}");
                    }
                    yield break;
                }
            }
        }



/*        public class MeetTheGamemasterDefinition : EntityDefinition<Adventure, AdventureConfig>
        {

            public MeetTheGamemasterDefinition()
            {
                Id = nameof(MeetTheGamemaster);
            }

            public override AdventureConfig GetConfig()
            {
                var ac = new AdventureConfig(
                                No: 696,
                                Id: Id,
                                HostId: "",
                                HostId2: "",
                                Music: 2,
                                HideUlt: false,
                                TempArt: true
                    );

                return ac;
            }

            public sealed class MeetTheGamemaster : Adventure
            {
                [RuntimeCommand("GetLegalCards", "")]
                public IEnumerator GetLegalCards()
                {
                    var legalCards = Library.EnumerateRollableCardTypes(10)
                        .SelectMany(vt => new Card[] { Library.CreateCard(vt.Item1), Library.CreateCard(vt.Item1), Library.CreateCard(vt.Item1) });

                    legalCards.Do(c => c.GameRun = GameRun);


                    SelectCardInteraction interaction = new SelectCardInteraction(0, legalCards.Count(), legalCards, SelectedCardHandling.DoNothing)
                    {
                        CanCancel = false,
                        Description = "deeznuts"
                    };


                    yield return base.GameRun.InteractionViewer.View(interaction);

                    //var cards = interaction.SelectedCards;
                    //base.Storage.SetValue("$pill", card.Id);
                    yield break;


                }
            }
        }*/


    }
}
