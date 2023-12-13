using HarmonyLib;
using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.StatusEffects;
using LBoL.Core.Units;
using LBoL.EntityLib.Exhibits.Common;
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
using System.Text;
using UnityEngine;
using static EffectLib.BepinexPlugin;

namespace EffectLib.StatusEffects
{

    public sealed class NoDmgLimitSeDef : BaseNoDmgLimitSeDef
    {
        public override IdContainer GetId() => nameof(NoDmgLimitSe);
    }

    [EntityLogic(typeof(NoDmgLimitSeDef))]
    public sealed class NoDmgLimitSe : BaseNoDmgLimitSe { }

    public abstract class BaseNoDmgLimitSeDef : StatusEffectTemplate
    {

        public override LocalizationOption LoadLocalization()
        {
            var gl = new GlobalLocalization(BepinexPlugin.embeddedSource);
            gl.LocalizationFiles.AddLocaleFile(Locale.En, "StatusEffectsEn.yaml");
            return gl;        
        }
        public override Sprite LoadSprite() => ResourceLoader.LoadSprite("NoDmgLimitSe.png", embeddedSource);

        public override StatusEffectConfig MakeConfig()
        {
            return new StatusEffectConfig(
                Id: "",
                Order: 10,
                Type: StatusEffectType.Special,
                IsVerbose: false,
                IsStackable: true,
                StackActionTriggerLevel: null,
                HasLevel: false,
                LevelStackType: StackType.Add,
                HasDuration: true,
                DurationStackType: StackType.Min,
                DurationDecreaseTiming: DurationDecreaseTiming.TurnStart,
                HasCount: false,
                CountStackType: StackType.Keep,
                LimitStackType: StackType.Keep,
                ShowPlusByLimit: false,
                Keywords: Keyword.None,
                RelativeEffects: new List<string>() { nameof(Graze) },
                VFX: "Default",
                VFXloop: "Default",
                SFX: "Default");
        }
    }






    public abstract class BaseNoDmgLimitSe : StatusEffect
    {

        protected override void OnAdded(Unit unit)
        {
            Duration = 1;
        }

        static public GameEventHandler<DamageEventArgs> WrapHandler(GameEventHandler<DamageEventArgs> handler)
        {
            return new GameEventHandler<DamageEventArgs>((DamageEventArgs args) => {


                DamageInfo inDmg = args.DamageInfo;

                handler(args);

                var noLimitSe = args.Target?.GetStatusEffectExtend<BaseNoDmgLimitSe>();

                if (noLimitSe != null)
                {
                    if (args.DamageInfo.Damage < inDmg.Damage)
                        args.DamageInfo = inDmg;

                    if (args._modifiers.FirstOrDefault(wr => wr.TryGetTarget(out var t) && t == noLimitSe) == null)
                    {
                        args.AddModifier(noLimitSe);
                    }
                }
            });
        }

        [HarmonyPatch]
        class Unit_Patch
        {
            static public ConditionalWeakTable<GameEvent<DamageEventArgs>, object> damageEventFilter = new ConditionalWeakTable<GameEvent<DamageEventArgs>, object>();


            static IEnumerable<MethodBase> TargetMethods()
            {
                yield return AccessTools.DeclaredConstructor(typeof(Unit));
            }


            static void Postfix(Unit __instance)
            {
                damageEventFilter.Add(__instance.DamageTaking, new object());
                damageEventFilter.Add(__instance.DamageReceiving, new object());

            }
        }


        // semi-unstable??
        [HarmonyPatch]
        [HarmonyPriority(Priority.Normal)]
        class GameEvent_Patch
        {

            static IEnumerable<MethodBase> TargetMethods()
            {
                yield return typeof(GameEvent<>).MakeGenericType(typeof(DamageEventArgs)).GetMethod("AddHandler", BindingFlags.Instance | BindingFlags.NonPublic);

            }


            static void Prefix(GameEvent<DamageEventArgs> __instance, ref GameEventHandler<DamageEventArgs> handler)
            {
                if (Unit_Patch.damageEventFilter.TryGetValue(__instance, out var _))
                {
                    handler = WrapHandler(handler);
                }
            }

        }


    }

    // removes exception throw if SE.Count is being set to less than 0
    [HarmonyPatch(typeof(StatusEffect), nameof(StatusEffect.Count), MethodType.Setter)]
    class SE_Count_Patch
    {

        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            return new CodeMatcher(instructions, generator)
                .MatchForward(false, new CodeMatch(OpCodes.Newobj, AccessTools.Constructor(typeof(ArgumentException), new Type[] { typeof(string) })))
                .Set(OpCodes.Call, AccessTools.Method(typeof(Debug), nameof(Debug.LogWarning), new Type[] { typeof(object) }))
                .Advance(1)
                .RemoveInstruction()
                .InsertAndAdvance(new CodeInstruction(OpCodes.Ldc_I4_0))
                .InsertAndAdvance(new CodeInstruction(OpCodes.Starg, 1))
                .InstructionEnumeration();
        }

    }




}
