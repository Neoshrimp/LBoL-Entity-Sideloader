using Cysharp.Threading.Tasks;
using HarmonyLib;
using LBoL.Base.Extensions;
using LBoL.Core;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Cards;
using LBoL.Presentation;
using LBoL.Presentation.I10N;
using LBoLEntitySideloader.ReflectionHelpers;
using MonoMod.Utils;
using System.Collections.Generic;
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