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


        [HarmonyPatch(typeof(Card.CardFormatWrapper), "FormatArgument")]
        class Card_Patch
        {
            static void Postfix(object arg, string format, ref string __result, Card.CardFormatWrapper __instance)
            {
                if (arg is DamageInfo dmgInfo && __instance._card is SuikaBigball suika && suika.Battle != null)
                {
                    //int num2 = __instance._card.Battle.CalculateDamage(__instance._card, __instance._card.Battle.Player, null, dmgInfo);
                    // 2do mirror ui colour issue
                    __result = GameEntityFormatWrapper.WrappedFormatNumber((int)suika.Damage.Damage, (int)dmgInfo.Damage, format);
                }
            }
        }



        [EntityLogic(typeof(SuikaBigballDefinition))]
        public sealed class SuikaBigball : Card
        {
            public DamageInfo UIDamage
            { 
                get
                {
                    var dmgInfo = base.Damage;
                    dmgInfo.Damage = Battle.CalculateDamage(this, Battle.Player, PendingTarget, dmgInfo);
                    return dmgInfo;

                }
            }

            public DamageInfo HalfDamage
            {
                get
                {


                    var dmgInfo = UIDamage;
                        
                    dmgInfo = dmgInfo.MultiplyBy(mult);

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
            float mult = 0.5f;

        }

    }
}
