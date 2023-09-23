using JetBrains.Annotations;
using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Battle;
using LBoL.Core.StatusEffects;
using LBoL.Core.Units;
using LBoL.Core;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using LBoLEntitySideloader.Entities;
using LBoLEntitySideloader;
using LBoLEntitySideloader.Resource;
using LBoLEntitySideloader.Attributes;

namespace Random_Examples
{
    public sealed class JojoCapExhibitDef : ExhibitTemplate
    {
        private IResourceSource embeddedSource;

        public override IdContainer GetId()
        {
            return nameof(JojoCapEx);
        }
        public override LocalizationOption LoadLocalization()
        {
            return new DirectLocalization(new Dictionary<string, object>() { { "Name", "Jojo Cap" }, { "Description", "You can feel the power surging!"} });
        }
        public override ExhibitSprites LoadSprite()
        {
            return null;
        }
        public override ExhibitConfig MakeConfig()
        {
            var exhibitConfig = new ExhibitConfig(
                Index: 0,
                Id: "",
                Order: 10,
                IsDebug: false,
                IsPooled: false,
                IsSentinel: false,
                Revealable: false,
                Appearance: AppearanceType.Nowhere,
                Owner: "",
                LosableType: ExhibitLosableType.CantLose,
                Rarity: Rarity.Common,
                Value1: 999,
                Value2: null,
                Value3: null,
                Mana: null,
                BaseManaRequirement: null,
                BaseManaColor: null,
                BaseManaAmount: 0,
                HasCounter: false,
                InitialCounter: 0,
                Keywords: Keyword.None,
                RelativeEffects: new List<string>() { },
                // example of referring to UniqueId of an entity without calling MakeConfig
                RelativeCards: new List<string>() { }
            );
            return exhibitConfig;
        }
        [EntityLogic(typeof(JojoCapExhibitDef))]
        [UsedImplicitly]
        public sealed class JojoCapEx : Exhibit, IMapModeOverrider
        {
            protected override void OnAdded(PlayerUnit player)
            {
                base.GameRun.AddMapModeOverrider(this);
            }
            protected override void OnRemoved(PlayerUnit player)
            {
                base.GameRun.RemoveMapModeOverrider(this);
            }
            public GameRunMapMode? MapMode
            {
                get
                {
                    return new GameRunMapMode?(GameRunMapMode.Free);
                }
            }
            public void OnEnteredWithMode()
            {
                base.NotifyActivating();
            }
            protected override void OnEnterBattle()
            {
                base.ReactBattleEvent<GameEventArgs>(base.Battle.BattleStarted, new EventSequencedReactor<GameEventArgs>(this.OnBattleStarted));
            }
            private IEnumerable<BattleAction> OnBattleStarted(GameEventArgs args)
            {
                base.NotifyActivating();
                yield return new ApplyStatusEffectAction<Firepower>(base.Owner, new int?(base.Value1), null, null, null, 0f, true);
                yield return new ApplyStatusEffectAction<Spirit>(base.Owner, new int?(base.Value1), null, null, null, 0f, true);
                yield break;
            }
        }
    }
}
