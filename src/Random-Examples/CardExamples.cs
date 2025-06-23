using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

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
        public Testkw(string kwSEid = nameof(ExtraTurn), bool isVerbose = false) : base(kwSEid, isVerbose)
        {
            descPos = KwDescPos.Last;
        }
    }

    [EntityLogic(typeof(AutoPlayHavocDef))]
    public sealed class AutoPlayHavoc : Card
    {

        public override void Initialize()
        {
            base.Initialize();
            this.AddCustomKeyword(ExampleKeywords.Testkw);
        }

        public override Interaction Precondition()
        {
            var cards = Battle.HandZone/*.Concat(Battle.DrawZone).Concat(Battle.DiscardZone)*/.Where(c => c != this);
            if (cards.FirstOrDefault() == null)
                return null;

            return new SelectCardInteraction(1, 1, cards);
        }

        protected override IEnumerable<BattleAction> Actions(UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
        {
            if (precondition is SelectCardInteraction selection)
            {
                yield return CardHelper.AutoCastAction(selection.SelectedCards.First(), UnitSelector.RandomEnemy, consumingMana, true);
            }
        }
    }
}
