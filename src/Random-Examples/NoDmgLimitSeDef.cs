using HarmonyLib;
using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Cards;
using LBoL.Core.StatusEffects;
using LBoL.Core.Units;
using LBoL.Presentation;
using LBoLEntitySideloader;
using LBoLEntitySideloader.Attributes;
using LBoLEntitySideloader.Entities;
using LBoLEntitySideloader.Resource;
using LBoLEntitySideloader.Utils;
using Spine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;
using static Random_Examples.BepinexPlugin;
using static UnityEngine.UI.GridLayoutGroup;

namespace Random_Examples
{
    public sealed class NoDmgLimitSeDef : StatusEffectTemplate
    {
        public override IdContainer GetId() => nameof(NoDmgLimitSe);

        public override LocalizationOption LoadLocalization() => new GlobalLocalization(BepinexPlugin.embeddedSource);
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
                RelativeEffects: new List<string>(),
                VFX: "Default",
                VFXloop: "Default",
                SFX: "Default");
        }
    }


    [HarmonyPatch(typeof(GuangxueMicai), nameof(GuangxueMicai.OnAdded))]
    class GuangxueMicai_Patch
    {

        static void Postfix(GuangxueMicai __instance)
        {
            log.LogDebug($"onadd byType: {__instance.Owner.GetStatusEffect<GuangxueMicai>()}");

            var actualNoLimtit = __instance.Owner.StatusEffects.FirstOrDefault(se => se.Id == nameof(GuangxueMicai));
            log.LogDebug($"onadd actual: {actualNoLimtit}");
        }
    }


    public sealed class deezCard : CardTemplate
    {
        public override IdContainer GetId() => nameof(deezCardLogic);


        public override CardImages LoadCardImages() => null;


        public override LocalizationOption LoadLocalization() => null;


        public override CardConfig MakeConfig() => DefaultConfig();

        [EntityLogic(typeof(deezCard))]
        public sealed class deezCardLogic : Card
        {
            public override void Initialize()
            {
                base.Initialize();
                log.LogDebug("CARD DEEZ 11");
            }
        }
    }




    


    [EntityLogic(typeof(NoDmgLimitSeDef))]
    public sealed class NoDmgLimitSe : StatusEffect
    {
        

        // handlers always trigger sequentially so it's probably ok to use single variable for dmg tracking
        DamageInfo innitialDmg;
        protected override void OnAdded(Unit unit)
        {
            Duration = 1;
            /*            HandleOwnerEvent(unit.DamageTaking, new GameEventHandler<DamageEventArgs>(this.TrackDmg), GameEventPriority.Highest);
                        HandleOwnerEvent(unit.DamageTaking, new GameEventHandler<DamageEventArgs>(this.AdjustDmg), GameEventPriority.Lowest);
                        HandleOwnerEvent(unit.DamageReceiving, new GameEventHandler<DamageEventArgs>(this.TrackDmg), GameEventPriority.Highest);
                        HandleOwnerEvent(unit.DamageReceiving, new GameEventHandler<DamageEventArgs>(this.AdjustDmg), GameEventPriority.Lowest);*/
            log.LogDebug($"onadd byType: {Owner.GetStatusEffect<NoDmgLimitSe>()}");

            var actualNoLimtit = Owner.StatusEffects.FirstOrDefault(se => se.Id == nameof(NoDmgLimitSe));
            log.LogDebug($"onadd actual: {actualNoLimtit}");
            log.LogDebug($"dez 4");


        }

        static public GameEventHandler<DamageEventArgs> WrapHandler(GameEventHandler<DamageEventArgs> handler)
        {
            return new GameEventHandler<DamageEventArgs>((DamageEventArgs args) => {
                
                
                DamageInfo inDmg = args.DamageInfo;
                handler(args);
                var noLimitSe = args.Target?.GetStatusEffect<NoDmgLimitSe>();
                log.LogDebug($"{args.Target}: {noLimitSe}");

                log.LogDebug($"extend {args.Target.GetStatusEffectExtend(typeof(NoDmgLimitSe))}");



                var byType = args.Target.StatusEffects.FirstOrDefault(se => se.GetType() == typeof(NoDmgLimitSe));
                log.LogDebug($"byType: {byType}");


                args.Target.StatusEffects.Do(se => log.LogDebug($"{se.Name}, {se.Id}, {se.GetType().FullName}"));

                if (noLimitSe != null) 
                {
                    if (args.DamageInfo.Damage < inDmg.Damage)
                        args.DamageInfo = inDmg;

                    if (args._modifiers.FirstOrDefault(wr => wr.TryGetTarget(out var t) && t as StatusEffect == noLimitSe) == null)
                    {
                        args.AddModifier(noLimitSe);
                    }
                }
            });
        }





        [HarmonyPatch]
        class StatusEffectHandler_Patch
        {

            static IEnumerable<MethodBase> TargetMethods()
            {
                var method = AccessTools.GetDeclaredMethods(typeof(StatusEffect)).Single(m => m.ToString().Contains(nameof(StatusEffect.HandleOwnerEvent)) && m.ToString().Contains(nameof(GameEventPriority)));
                yield return method.MakeGenericMethod(typeof(DamageEventArgs));
            }


            static void Prefix(StatusEffect __instance, GameEvent<DamageEventArgs> @event, ref GameEventHandler<DamageEventArgs> handler)
            {

                if (__instance.Owner != null && (@event == __instance.Owner.DamageTaking || @event == __instance.Owner.DamageReceiving))
                {
                    log.LogDebug(__instance.Owner);
                    handler = WrapHandler(handler);
                }
            }

        }



/*        private void TrackDmg(DamageEventArgs args)
        {
            innitialDmg = args.DamageInfo;
        }

        private void AdjustDmg(DamageEventArgs args)
        {
            if (args.DamageInfo.Damage < innitialDmg.Damage)
            {
                NotifyActivating();
                args.DamageInfo = innitialDmg;
                args.AddModifier(this);
            }
        }*/

    }
}
