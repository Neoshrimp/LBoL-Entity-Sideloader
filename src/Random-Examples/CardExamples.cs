using HarmonyLib;
using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Battle.Interactions;
using LBoL.Core.Cards;
using LBoL.Core.StatusEffects;
using LBoL.EntityLib.Cards.Character.Sakuya;
using LBoL.Presentation;
using LBoLEntitySideloader;
using LBoLEntitySideloader.Attributes;
using LBoLEntitySideloader.CustomKeywords;
using LBoLEntitySideloader.Entities;
using LBoLEntitySideloader.ExtraFunc;
using LBoLEntitySideloader.Resource;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

namespace Random_Examples
{
    [OverwriteVanilla]
    public class TimeWalkCardDef : CardTemplate
    {
        public override IdContainer GetId() => nameof(TimeWalk);


        public override CardImages LoadCardImages()
        {
            var imgs = new CardImages(BepinexPlugin.embeddedSource);
            imgs.main = (Texture2D)ResourcesHelper.TryGetCardImage(nameof(TimeWalk));
            return imgs;

        }

        [DontOverwrite]
        public override LocalizationOption LoadLocalization() => null;

        [DontOverwrite]
        public override CardConfig MakeConfig() => null;

    }

    public sealed class AutoPlayHavocDef : CardTemplate
    {
        public override IdContainer GetId()
        {
            return nameof(AutoPlayHavoc);
        }

        public override CardImages LoadCardImages()
        {
            return null;
        }

        public override LocalizationOption LoadLocalization()
        {
            return null;
        }

        public override CardConfig MakeConfig()
        {
            var con = DefaultConfig();
            con.Type = LBoL.Base.CardType.Skill;
            con.TargetType = TargetType.Nobody;
            con.Colors = new ManaColor[] { ManaColor.Red };
            con.Cost = new ManaGroup() { Any = 2 };

            return con;
        }
    }


    public static class ExampleKeywords
    {
        public static CardKeyword SomeKw => new CardKeyword(nameof(ExtraTurn));

        public static Testkw Testkw = new Testkw();
    }

    public class Testkw : CardKeyword
    {
        public Testkw(string kwSEid = nameof(TestKwSe), bool isVerbose = false) : base(kwSEid, isVerbose)
        {
            descPos = KwDescPos.Last;
            hasExtendedKeywordName = true;
        }


    }


    public sealed class TestKwSeDef : StatusEffectTemplate
    {
        public override IdContainer GetId() => nameof(TestKwSe);

        public override LocalizationOption LoadLocalization() => new GlobalLocalization(BepinexPlugin.embeddedSource);

        public override Sprite LoadSprite() => null;

        public override StatusEffectConfig MakeConfig() => this.DefaultConfig();
    }

    [EntityLogic(typeof(TestKwSeDef))]
    public sealed class TestKwSe : StatusEffect, IExtendedKeywordName
    {

        public string SecretNumber
        {
            get
            {
                if(SourceCard is AutoPlayHavoc havoc)
                    return havoc.secretNumber.ToString();
                return "";
            }
        }

        public string ExtendedKeywordName(Card card)
        {
            SourceCard = card;
            return LocalizeProperty("ExtendedName", true, true).RuntimeFormat(this);

            BepinexPlugin.log.LogDebug("deez");
            var rez = this.LocalizeProperty("SpecialName", true);
            if (card is AutoPlayHavoc havoc)
                rez = $"{rez} <{havoc.secretNumber}> <sprite=\"Point\" name=\"Power\">";
            return rez;
        }
    }

    [EntityLogic(typeof(AutoPlayHavocDef))]
    public sealed class AutoPlayHavoc : Card
    {
        public int secretNumber = -69;

        public override void Initialize()
        {
            base.Initialize();
            if(Battle != null)
                secretNumber = UnityEngine.Random.Range(0, 100);
            this.AddCustomKeyword(ExampleKeywords.Testkw);
        }

        public override Interaction Precondition()
        {
            var cards = Battle.HandZone/*.Concat(Battle.DrawZone).Concat(Battle.DiscardZone)*/.Where(c => c != this);
            if (cards.FirstOrDefault() == null)
                return null;

            return new SelectCardInteraction(1, 1, cards);
        }

        protected override void OnEnterBattle(BattleController battle)
        {
            ReactBattleEvent<CardUsingEventArgs>(Battle.CardUsed, new EventSequencedReactor<CardUsingEventArgs>(this.OnCardUsed));
        }

        private IEnumerable<BattleAction> OnCardUsed(CardUsingEventArgs args)
        {
            if (!this.Battle.BattleShouldEnd)
            {
                Card card = args.Card;
                if (card.CardType == CardType.Friend && !card.Summoning && card.UltimateUsed == true)
                {
                    this.NotifyActivating();
                    var tokenCard = card.CloneTwiceToken();
                    tokenCard.UltimateUsed = false;
                    tokenCard._loyalty -= card.UltimateCost;
                    tokenCard.IsPlayTwiceToken = true;

                    tokenCard.PlayTwiceSourceCard = this; //adapt deez

                    yield return new PlayTwiceAction(tokenCard, args);
                }
            }
        }





        [HarmonyPatch]
        //[HarmonyDebug]
        class Hijack_CardPrecond_Patch
        {

            static Type _delegateType = null;

            public static Type DelegateType
            {
                get
                {
                    if(_delegateType == null)
                        _delegateType = typeof(PlayCardAction).GetNestedTypes(AccessTools.all).First(t => t.Name.Contains("DisplayClass22"));
                    return _delegateType;
                }
            }

            static IEnumerable<MethodBase> TargetMethods()
            {
                yield return AccessTools.GetDeclaredMethods(DelegateType).First(m => m.Name.Contains("<GetPhases>b__1"));

            }


            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                var fi_this = AccessTools.GetDeclaredFields(DelegateType).First(m => m.Name.Contains("this"));

                return new CodeMatcher(instructions)
                    .MatchEndForward(new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(PlayCardAction), nameof(PlayCardAction._precondition))))
                    .Advance(1)
                    .InsertAndAdvance(new CodeInstruction(OpCodes.Ldarg_0))
                    .InsertAndAdvance(new CodeInstruction(OpCodes.Ldfld, fi_this))
                    .InsertAndAdvance(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Hijack_CardPrecond_Patch), nameof(Hijack_CardPrecond_Patch.ModPrecond))))
                    .InstructionEnumeration();
            }

            private static Interaction ModPrecond(Interaction ogInteraction, PlayCardAction action)
            {
                if (action.Args.Card?.PlayTwiceSourceCard is AutoPlayHavoc)
                {
                    var ultFound = false;
                    if (ogInteraction is MiniSelectCardInteraction miniSelectCardInteraction)
                    {
                        var ultCard = miniSelectCardInteraction.PendingCards.FirstOrDefault(c => c.FriendToken == FriendToken.Ultimate);
                        if (ultCard != null)
                            ultFound = true;
                        miniSelectCardInteraction.SelectedCard = ultCard;
                    }
                    // ig you dont really need this
                    else if (ogInteraction is SelectCardInteraction selectCardInteraction)
                    {
                        var ultCard = selectCardInteraction.PendingCards.FirstOrDefault(c => c.FriendToken == FriendToken.Ultimate);
                        if (ultCard != null)
                            ultFound = true;
                        selectCardInteraction.SelectedCards = new List<Card>() { ultCard };
                    }

                    if (ultFound)
                    { 
                        ogInteraction.Source = action.Args.Card;
                        return null;
                    }
                }
                return ogInteraction;
            }
        }



        protected override IEnumerable<BattleAction> Actions(UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
        {
            if (precondition is SelectCardInteraction selection)
            {
                //yield return CardHelper.AutoCastAction(selection.SelectedCards.First(), UnitSelector.RandomEnemy, consumingMana, true);
            }
            secretNumber = UnityEngine.Random.Range(0, 100);
            yield break;
        }
    }
}
