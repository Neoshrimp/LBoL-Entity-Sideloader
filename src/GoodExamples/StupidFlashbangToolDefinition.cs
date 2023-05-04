using LBoL.ConfigData;
using LBoL.Core.Cards;
using LBoLEntitySideloader;
using LBoLEntitySideloader.Attributes;
using LBoLEntitySideloader.Entities;
using LBoLEntitySideloader.Resource;
using System;
using System.Collections.Generic;
using System.Text;
using static GoodExamples.BepinexPlugin;
using UnityEngine;
using LBoL.Core;
using LBoL.Base;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.Base.Extensions;
using System.Collections;
using LBoL.Presentation;
using LBoL.Core.Units;
using LBoL.EntityLib.EnemyUnits.Character;
using LBoL.EntityLib.Adventures.Shared12;
using System.Linq;

namespace GoodExamples
{
    public sealed class StupidFlashbangToolDefinition : CardTemplate
    {
        public override IdContainer GetId()
        {
            return nameof(StupidFlashbangTool);
        }

        public override CardImages LoadCardImages()
        {
            var imgs = new CardImages(embeddedSource);
            imgs.AutoLoad(this, ".png", relativePath: "Flashbang.");
            return imgs;
        }

        public override LocalizationOption LoadLocalization()
        {
            return new GlobalLocalization(embeddedSource);
        }

        public override CardConfig MakeConfig()
        {
            var cardConfig = new CardConfig(
                    Index: sequenceTable.Next(typeof(CardConfig)),
                    Id: "",
                    Order: 10,
                    AutoPerform: true,
                    // perform BirdSing sfx with 0.2s delay and 6 CameraFlash vfx one after the other with 0.2 seconds in between. Delaying vfx and sfx in perform array is sideloader feature.
                    Perform: new string[7][] { new string[] { "4", "BirdSing", "0.2" },   new string[] { "2", "CameraFlash", "0.2" }, new string[] { "2", "CameraFlash", "0.4"}, new string[] { "2", "CameraFlash", "0.6" }, new string[] { "2", "CameraFlash", "0.8" }, new string[] { "2", "CameraFlash", "1" }, new string[] { "2", "CameraFlash", "1.2" } },
                    GunName: "Simple1",
                    GunNameBurst: "Simple1",
                    DebugLevel: 0,
                    Revealable: false,
                    IsPooled: true,
                    HideMesuem: false,
                    IsUpgradable: false,
                    Rarity: Rarity.Rare,
                    Type: CardType.Tool,
                    // target type All doesn't work
                    TargetType: TargetType.AllEnemies,
                    Colors: new List<ManaColor>() { },
                    IsXCost: false,
                    Cost: new ManaGroup() { },
                    UpgradedCost: null,
                    MoneyCost: null,
                    Damage: 1,
                    UpgradedDamage: null,
                    Block: null,
                    UpgradedBlock: null,
                    Shield: null,
                    UpgradedShield: null,
                    Value1: 3,
                    UpgradedValue1: null,
                    Value2: null,
                    UpgradedValue2: null,
                    Mana: null,
                    UpgradedMana: null,
                    Scry: null,
                    UpgradedScry: null,
                    ToolPlayableTimes: 1,

                    Keywords: Keyword.Replenish | Keyword.Exile | Keyword.Accuracy,
                    UpgradedKeywords: Keyword.Replenish | Keyword.Exile | Keyword.Accuracy,
                    EmptyDescription: false,
                    RelativeKeyword: Keyword.Overdraft,
                    UpgradedRelativeKeyword: Keyword.Overdraft,

                    RelativeEffects: new List<string>() { },
                    UpgradedRelativeEffects: new List<string>() { },
                    RelativeCards: new List<string>() { },
                    UpgradedRelativeCards: new List<string>() { },
                    Owner: null,
                    Unfinished: false,
                    Illustrator: null,
                    SubIllustrator: new List<string>() { }
                );

            return cardConfig;
        }

        [EntityLogic(typeof(StupidFlashbangToolDefinition))]
        public sealed class StupidFlashbangTool : Card
        {
            protected override IEnumerable<BattleAction> Actions(UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
            {
                // target player as well TargetType.All is bugged
                yield return AttackAction(new Unit[] { Battle.Player }.Concat(selector.GetUnits(Battle)));

                foreach (var enemy in selector.GetEnemies(Battle))
                {
                    // janky code modifying stuff directly which wasn't meant to be modified directly
                    // there might be some odd combination of circumstances (enemy trying to run way, double action etc.) where this breaks
                    enemy._turnMoves.Clear(); // actual actions the enemy is going to do
                    enemy.ClearIntentions(); // visual intention icons
                    var stun = Intention.Stun();
                    stun.Source = enemy;
                    enemy._turnMoves.Add(new SimpleEnemyMove(stun, new EnemyMoveAction[] {
                        new EnemyMoveAction(enemy, "Blinded", true)
                    }));
                    enemy.Intentions.Add(stun);
                    // redraw intention icons
                    enemy.NotifyIntentionsChanged();
                }


                yield return new LockRandomTurnManaAction(Value1);
                yield return new RequestEndPlayerTurnAction();
            }

        }
    }
}
