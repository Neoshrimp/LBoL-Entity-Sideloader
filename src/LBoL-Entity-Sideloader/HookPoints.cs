﻿using BepInEx;
using BepInEx.Configuration;
using Cysharp.Threading.Tasks;
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
using LBoLEntitySideloader.ReflectionHelpers;
using MonoMod.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Untitled;
using Untitled.ConfigDataBuilder;
using Untitled.ConfigDataBuilder.Base;
using Debug = UnityEngine.Debug;

namespace LBoLEntitySideloader
{

    internal class HookPoints
    {
        private static readonly BepInEx.Logging.ManualLogSource log = BepinexPlugin.log;





        /// <summary>
        /// hook after all the vanilla configs, entities, assets and localization have been loaded. Can be used to specifically modify vanilla properties
        /// </summary>
        [HarmonyPatch(typeof(GameEntry), nameof(GameEntry.InitializeRestAsync))]
        [HarmonyPriority(Priority.First)]
        class GameEntry_Patch
        {
            static public async void Postfix(Task __result)
            {   
                await __result;

                EntityManager.Instance.LoadAll();

            }

        }





        //[HarmonyPatch(typeof(L10nManager), nameof(L10nManager.ReloadAsync))]
        [HarmonyPatch(typeof(L10nManager), nameof(L10nManager.SetLocaleAsync))]
        [HarmonyPriority(Priority.First)]
        class Localization_Patch
        {
            static async UniTask Postfix(UniTask __result)
            {
                await UniTask.WhenAll(new UniTask[] { __result });
                log.LogDebug("loc reload");
                EntityManager.Instance.LoadLocalization();
            }
        }

        //[HarmonyPatch]
        class Loc_IntrusivePatch
        {

            static IEnumerable<MethodBase> TargetMethods()
            {

                yield return ExtraAccess.InnerMoveNext(typeof(L10nManager), nameof(L10nManager.ReloadAsync));
            }



            static void LoadLocWrap()
            {
                EntityManager.Instance.LoadLocalization();
            }

            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
            {
                foreach (var ci in instructions)
                {
                    if (ci.opcode == OpCodes.Call && (MethodInfo)ci.operand == AccessTools.Method(typeof(CrossPlatformHelper), nameof(CrossPlatformHelper.SetWindowTitle)))
                    {
                        yield return ci;
                        yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Loc_IntrusivePatch), nameof(LoadLocWrap)));
                    }
                    else
                    {
                        yield return ci;
                    }
                }
            }

        }







        [HarmonyPatch]
        class ReadVanillaConfigIds_Patch
        {

            static IEnumerable<MethodBase> TargetMethods()
            {
                foreach (var t in ConfigReflection.AllConfigTypes(exclude: false))
                {
                    yield return AccessTools.Method(t, "Load");
                }
            }


            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, MethodBase original)
            {
                foreach (var ci in instructions)
                {
                    if (ci.Is(OpCodes.Newobj, AccessTools.FirstConstructor(original.DeclaringType, c => c.GetParameters().Count() > 0)))
                    {
                        //log.LogDebug($"injected at {original.DeclaringType.Name}");

                        yield return ci;
                        yield return new CodeInstruction(OpCodes.Dup);
                        yield return new CodeInstruction(OpCodes.Ldc_I4_1);
                        yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(UniqueTracker), nameof(UniqueTracker.TrackVanillaConfig)));
                    }
                    else
                    {
                        yield return ci;
                    }
                }
            }


        }


        /// <summary>
        /// Makes vfx and sfx optionally delayed in perform array
        /// </summary>
        [HarmonyPatch(typeof(Card), nameof(Card.CardPerformAction))]
        class CardPerformAction_Patch
        {

            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
            {
                int i = 0;
                var ciList = instructions.ToList();
                var c = ciList.Count();
                CodeInstruction prevCi = null;
                foreach (var ci in instructions)
                {
                    if (ci.Is(OpCodes.Ldc_R4, 0f) && (ciList.ElementAtOrDefault(i + 5)?.Is(OpCodes.Call, AccessTools.Method(typeof(PerformAction), nameof(PerformAction.Effect))) ?? false))
                    {
                        yield return new CodeInstruction(OpCodes.Ldloc_2);
                    }

                    else if (ci.Is(OpCodes.Ldc_R4, 0f) && (ciList.ElementAtOrDefault(i + 1)?.Is(OpCodes.Call, AccessTools.Method(typeof(PerformAction), nameof(PerformAction.Sfx))) ?? false))
                    {
                    
                        yield return new CodeInstruction(OpCodes.Ldloc_2);

                    }
                    else
                    {
                        yield return ci;
                    }
                    prevCi = ci;
                    i++;
                }
            }

            }




    }
}