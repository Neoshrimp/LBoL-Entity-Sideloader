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
        class GameEntry_Patch
        {

            static public async void Postfix(Task __result)
            {   
                await __result;

                EntityManager.Instance.LoadAll();

            }

        }



        // temp fix?
        [HarmonyPatch(typeof(CrossPlatformHelper), nameof(CrossPlatformHelper.SetWindowTitle))]
        class Localization_Patch
        {

            static void Postfix()
            {
                try
                {
                    log.LogDebug("loc reload");
                    EntityManager.Instance.LoadLocalization();
                }
                catch (Exception e)
                {

                    log.LogWarning(e);
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


        [HarmonyPatch(typeof(GameRunController), nameof(GameRunController.BaseCardWeight))]
        class UncapColorLimitation_Patch
        {

            // cba'd to transpile jump table
            static bool Prefix(ref float __result, CardConfig config, bool applyFactors, GameRunController __instance)
            {

                int trivialColorCount = Math.Max(config.Colors.Count, config.Cost.TrivialColorCount);
                if (trivialColorCount <= 3)
                    return true;

                float num;

                switch (trivialColorCount)
                {
                    case 4:
                        num = 1.3f;
                        break;
                    case 5:
                        num = 1.4f;
                        break;
                    default:
                        throw new InvalidDataException($"{trivialColorCount} is too many colors in either {config.Cost} or {config.Colors} of card {config.Id}");
                }


                float num3 = num;
                float num4 = 1f;
                int count = config.Colors.Count;
                if (count <= 0)
                {
                    if (count == 0)
                    {
                        num4 = 0.8f;
                    }
                }
                else
                {
                    foreach (ManaColor manaColor in config.Colors)
                    {
                        float num5 = (float)__instance.BaseMana.GetValue(manaColor) / (float)__instance.BaseMana.Amount;
                        num5 -= 0.5f;
                        num5 *= 0.8f;
                        num4 += num5;
                    }
                    num4 = Math.Max(num4, 0.8f);
                }
                num3 *= num4;
                if (applyFactors)
                {
                    if (config.Rarity == Rarity.Rare)
                    {
                        num3 *= __instance._cardRareWeightFactor;
                    }
                    float num6;
                    if (__instance._cardRewardWeightFactors.TryGetValue(config.Id, out num6))
                    {
                        num3 *= num6;
                    }
                }
                __result = num3;

                return false;
            }


            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
            {
                int i = 0;
                var ciList = instructions.ToList();
                var c = ciList.Count();
                CodeInstruction prevCi = null;
                int patchCount = 0;
                bool skipMode = false;
                CodeInstruction jmpSwitchEnd = null;
                CodeInstruction numStore = null;

                int patchCasesTotal = 2;
                foreach (var ci in instructions)
                {
                    if (ci.Is(OpCodes.Ldstr, "Invalid cost pattern {0} of card '{1}'") && patchCount < patchCasesTotal)
                    {
                        skipMode = true;
                        jmpSwitchEnd = prevCi;
                        numStore = ciList[i - 2];
                        // prevents deletion of instruction label
                        yield return ci;
                        yield return new CodeInstruction(OpCodes.Pop);
                    }
                    else if (skipMode && ci.opcode == OpCodes.Throw && patchCount < patchCasesTotal)
                    {
                        // trivialColorCount case 1
                        if (patchCount == 0)
                        {
                            yield return new CodeInstruction(OpCodes.Ldc_R4, 1.2f);
                        }
                        // trivialColorCount case 2
                        else if (patchCount == 1)
                        {
                            yield return new CodeInstruction(OpCodes.Ldc_R4, 1.2f);

                        }
                        yield return numStore;
                        yield return jmpSwitchEnd;

                        skipMode = false;
                        patchCount += 1;
                    }
                    else if (!skipMode)
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