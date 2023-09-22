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
using LBoL.Presentation.Units;
using LBoLEntitySideloader.Entities;
using LBoLEntitySideloader.ReflectionHelpers;
using MonoMod.Utils;
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


                EntityManager.Instance.LoadAll(EntityManager.Instance.sideloaderUsers, loadLoc: false);

                UniqueTracker.Instance.RaisePostMainLoad();
                // secondary users are populate after RaisePostMainLoad
                EntityManager.Instance.LoadAll(EntityManager.Instance.secondaryUsers, loadLoc: true);

                UniqueTracker.Instance.populateLoadoutInfosActions.Do(a => a.Invoke());

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

                // untested
                EntityManager.Instance.LoadLocalization(EntityManager.Instance.sideloaderUsers);
                UniqueTracker.Instance.RaisePostMainLoad();
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