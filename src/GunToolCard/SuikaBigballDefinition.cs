using HarmonyLib;
using LBoL.Base;
using LBoL.Base.Extensions;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Cards;
using LBoL.Core.StatusEffects;
using LBoL.Core.Units;
using LBoLEntitySideloader;
using LBoLEntitySideloader.Attributes;
using LBoLEntitySideloader.Entities;
using LBoLEntitySideloader.Resources;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;
using static GunToolCard.Plugin;
using static UnityEngine.GraphicsBuffer;

namespace GunToolCard
{



    [OverwriteVanilla]
    public sealed class SuikaBigballDefinition : CardTemplate
    {
        public override IdContainer GetId()
        {
            return nameof(LBoL.EntityLib.Cards.Neutral.Red.SuikaBigball);
        }

        public override CardImages LoadCardImages()
        {
            var cardImages = new CardImages(embeddedSource);
            cardImages.AutoLoad(this, ".png");
            return cardImages;
        }

        public override LocalizationOption LoadText()
        {
            return new GlobalLocalization();
        }

        public override CardConfig MakeConfig()
        {
            return CardConfig.FromId(GetId());
        }

        public class StringWrap
        {
            public string s;
        }

        public static ConditionalWeakTable<GameEntityFormatWrapper, StringWrap> wt_PropertyName = new ConditionalWeakTable<GameEntityFormatWrapper, StringWrap>();

        // bunch of bullshit to make tooltip damage number display correctly
        // removes general damage calculation for SuikaBigball since it's already done in the properties
        [HarmonyPatch(typeof(Card.CardFormatWrapper), "FormatArgument")]
        class CardFormatWrapper_Patch
        {
            static bool Prefix(object arg, string format, ref string __result, Card.CardFormatWrapper __instance)
            {
                if (arg is DamageInfo dmgInfo && __instance._card is SuikaBigball suika && suika.Battle != null)
                {
                    if (!wt_PropertyName.TryGetValue(__instance, out StringWrap stringWrap))
                    {
                        log.LogWarning($"SuikaBigball: {__instance} does not have property name associated for description formatting");
                    }
                    var propName = stringWrap?.s;
                    log.LogDebug($"propName: {propName}");

                    // getter name is required to know if formatter is formatting main or shochwave dmg number. Depending on difference between base dmg and actual damage number will be coloured differently
                    var baseDmg = propName == null || propName == nameof(SuikaBigball.UIDamage) ? (int)suika.Damage.Damage : (int)(suika.Damage.Damage * suika.mult);


                    __result = GameEntityFormatWrapper.WrappedFormatNumber(baseDmg, (int)dmgInfo.Damage, format);
                    return false;
                }
                return true;
            }
        }



        // passes the getter name information down the call chain
        [HarmonyPatch]
        class GameEntityFormatWrapper_Patch
        {

            static IEnumerable<MethodBase> TargetMethods()
            {
                yield return AccessTools.Method(typeof(Card.CardFormatWrapper), "Format");
            }


            [HarmonyPrefix]
            static void FormatPrefix(string key, Card.CardFormatWrapper __instance)
            {

                if (__instance._card is SuikaBigball suika && suika.Battle != null)
                {

                    wt_PropertyName.AddOrUpdate(__instance, new StringWrap() { s = key});
                    
                }

            }
        }





        [EntityLogic(typeof(SuikaBigballDefinition))]
        public sealed class SuikaBigball : Card
        {
            // only for being displayed in description
            public DamageInfo UIDamage
            { 
                get
                {
                    var dmgInfo = base.Damage;
                    // this method predicts what the damage would be given all player modifiers (firepower, burst etc) AND target modifiers like cammo or vuln
                    if(Battle != null)
                        dmgInfo.Damage = Battle.CalculateDamage(this, Battle.Player, PendingTarget, dmgInfo);
                    return dmgInfo;

                }
            }

            // only for being displayed in description
            public DamageInfo HalfDamage
            {
                get
                {
                    var dmgInfo = UIDamage;
                    dmgInfo = dmgInfo.MultiplyBy(mult);
                    // this only takes player modifiers into account. Exact damage of shockwave cannot be displayed as a single number as it targets many units and they all could have different modifiers. Hence, the target modifiers should not be considered.
                    if (Battle != null)
                        dmgInfo.Damage = Battle.CalculateDamage(this, Battle.Player, null, dmgInfo);
                    return  dmgInfo;
                }
            }

            protected override void OnEnterBattle(BattleController battle)
            {
                base.OnEnterBattle(battle);
                

                ReactBattleEvent(Battle.Player.DamageDealt, new EventSequencedReactor<DamageEventArgs>(OnPlayerlDamageDealt));


            }

            private IEnumerable<BattleAction> OnPlayerlDamageDealt(DamageEventArgs args)
            {
                if (args.ActionSource == this && args.DamageInfo.DamageType == DamageType.Attack && canTriggerShockwave)
                {
                    NotifyActivating();

                    var shockwaveTargets = base.Battle.EnemyGroup.Alives.Where((EnemyUnit enemy) => enemy != originalTarget).Cast<Unit>().ToList<Unit>();
                    originalTarget = null;
                    canTriggerShockwave = false;
                    var dmgInfo = args.DamageInfo;

                    yield return base.AttackAction(shockwaveTargets, "Instant", new DamageInfo(dmgInfo.Damage * mult, dmgInfo.DamageType, false, this.IsAccuracy));


                }
            }

            protected override IEnumerable<BattleAction> Actions(UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
            {

                yield return PerformAction.Chat(Battle.Player, "SUIKA DEEEEEZ!!!", 3f, 0.5f, 0f, true);
                originalTarget = selector.GetEnemy(base.Battle);
                canTriggerShockwave = true;
                yield return base.AttackAction(selector);
                yield return base.DebuffAction<FirepowerNegative>(base.Battle.Player, base.Value1, 0, 0, 0, true, 0.2f);


            }

            Unit originalTarget;
            bool canTriggerShockwave = true;
            public readonly float mult = 0.5f;

        }

    }
}
