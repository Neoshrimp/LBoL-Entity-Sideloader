using Cysharp.Threading.Tasks;
using HarmonyLib;
using LBoL.Base;
using LBoL.Base.Extensions;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Cards;
using LBoL.Core.StatusEffects;
using LBoL.Presentation;
using LBoL.Presentation.I10N;
using LBoL.Presentation.UI.Panels;
using LBoL.Presentation.Units;
using LBoLEntitySideloader.Entities;
using LBoLEntitySideloader.ReflectionHelpers;
using MonoMod.Utils;
using Spine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading.Tasks;

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
        class InitializeRestAsync_Patch
        {

            static public async void Postfix(Task __result)
            {   
                await __result;


                EntityManager.Instance.LoadAll(EntityManager.Instance.sideloaderUsers, "All primary Sideloader users registered!", "Finished loading primary user resources", loadLoc: false);

                UniqueTracker.Instance.RaisePostMainLoad();
                // secondary users are populate after RaisePostMainLoad
                EntityManager.Instance.LoadAll(EntityManager.Instance.secondaryUsers, "All secondary Sideloader users registered!", "Finished loading secondary user resources", loadLoc: true);

                UniqueTracker.Instance.populateLoadoutInfosActions.Do(a => a.Invoke());

                EntityManager.Instance.PostAllLoadProcessing();

            }

        }





        // temp fix?
        [HarmonyPatch(typeof(CrossPlatformHelper), nameof(CrossPlatformHelper.SetWindowTitle))]
        [HarmonyPriority(Priority.First)]
        class Localization_Patch
        {

            static void Postfix()
            {
                try
                {
                    log.LogDebug("loc reload");
                    EntityManager.Instance.LoadLocalization(EntityManager.Instance.sideloaderUsers);
                    EntityManager.Instance.LoadLocalization(EntityManager.Instance.secondaryUsers);

                }
                catch (Exception e)
                {
                    log.LogWarning(e);
                }
            }
        }



        [HarmonyPatch(typeof(GameDirector), nameof(GameDirector.Awake))]
        [HarmonyPriority(Priority.First)]
        class AddFormations_Patch
        {
            static void Postfix()
            {
                EnemyGroupTemplate.LoadCustomFormations();
            }
        }


        [HarmonyPatch(typeof(LBoL.Presentation.Environment), nameof(LBoL.Presentation.Environment.Awake))]
        [HarmonyPriority(Priority.First)]
        class AddEnvironments_Patch
        {
            static void Postfix()
            {
                StageTemplate.LoadCustomEnvironments();
            }
        }



        // formation reload needs to be delayed after enemies have cleared
        [HarmonyPatch(typeof(GameDirector), nameof(GameDirector.InternalClearEnemies))]
        class FormationsHotReload_Patch
        {
            static void Postfix()
            {
                if (BepinexPlugin.doingMidRunReload > 0)
                {
                    BepinexPlugin.doingMidRunReload--;
                    EnemyGroupTemplate.ReloadFormations();
                }

            }
        }



        [HarmonyPatch]
        class SpellPanelSpecialLoc_Patch
        {
            static IEnumerable<MethodBase> TargetMethods()
            {
                yield return ExtraAccess.InnerMoveNext(typeof(SpellPanel), nameof(SpellPanel.CustomLocalizationAsync));
            }

            static void LoadLoc(SpellPanel spellPanel)
            {
                SpellTemplate.LoadAllSpecialLoc(spellPanel);
            }

            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
            {
                var l10nF = AccessTools.Field(typeof(SpellPanel), nameof(SpellPanel._l10nTable));
                return new CodeMatcher(instructions, generator)
                    .End()
                    .MatchBack(false, new CodeInstruction(OpCodes.Stfld, l10nF))
                    .Advance(-1)
                    .MatchBack(false, new CodeInstruction(OpCodes.Stfld, l10nF))
                    .Advance(1)
                    .Insert(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(SpellPanelSpecialLoc_Patch), nameof(LoadLoc))))
                    .Insert(new CodeInstruction(OpCodes.Ldloc_1))
                    .InstructionEnumeration();
            }


        }



        // 2do fix this
        //[HarmonyPatch(typeof(L10nManager), nameof(L10nManager.ReloadAsync))]
        //[HarmonyPatch(typeof(L10nManager), nameof(L10nManager.SetLocaleAsync))]
        /*        [HarmonyPriority(Priority.First)]
                class Localization_Patch
                {


                    static async void Postfix(UniTask __result)
                    {
                        try
                        {

                            await UniTask.WhenAll(new UniTask[] { __result });
                            log.LogDebug("loc reload");
                            EntityManager.Instance.LoadLocalization();
                        }
                        catch (Exception e)
                        {

                            log.LogWarning(e);
                        }
                    }
                }*/


        //[HarmonyPatch]
        class Loc_IntrusivePatch
        {

            static IEnumerable<MethodBase> TargetMethods()
            {

                yield return ExtraAccess.InnerMoveNext(typeof(L10nManager), nameof(L10nManager.ReloadAsync));
            }



            static void LoadLocWrap()
            {

                EntityManager.Instance.LoadLocalization(EntityManager.Instance.sideloaderUsers);
                EntityManager.Instance.LoadLocalization(EntityManager.Instance.secondaryUsers);

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




    }
}