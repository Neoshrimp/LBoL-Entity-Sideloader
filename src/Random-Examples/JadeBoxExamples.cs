using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Cards;
using LBoL.Core.StatusEffects;
using LBoL.Core.Units;
using LBoL.EntityLib.Cards.Neutral.Red;
using LBoL.EntityLib.Exhibits.Shining;
using LBoL.EntityLib.JadeBoxes;
using LBoL.Presentation.UI.Panels;
using LBoLEntitySideloader;
using LBoLEntitySideloader.Attributes;
using LBoLEntitySideloader.Entities;
using LBoLEntitySideloader.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using static Random_Examples.BepinexPlugin;

namespace Random_Examples
{
    public class JadeBoxExamples
    {
        public sealed class GetManaJadeboxDef : JadeBoxTemplate
        {
            public override IdContainer GetId()
            {
                return nameof(GetManaJadeBox);
            }

            public override LocalizationOption LoadLocalization()
            {
                var gl = new GlobalLocalization(embeddedSource);
                gl.LocalizationFiles.AddLocaleFile(Locale.En, "JadeBoxEn");
                return gl;
            }

            public override JadeBoxConfig MakeConfig()
            {
                var config = DefaultConfig();
                config.Mana = new ManaGroup() { Philosophy = 2 };
                return config;
            }



            [EntityLogic(typeof(GetManaJadeboxDef))]
            public sealed class GetManaJadeBox : JadeBox
            {
                protected override void OnEnterBattle()
                {
                    ReactBattleEvent(Battle.Player.TurnStarted, OnBattleStarted);
                }

                private IEnumerable<BattleAction> OnBattleStarted(GameEventArgs args)
                {
                    if (Battle.Player.TurnCounter == 1)
                    { 
                        NotifyActivating();
                        yield return new GainManaAction(this.Mana);
                    }
                }


            }
        }

        [OverwriteVanilla]
        public sealed class BigFinkgDeckJadeboxDef : JadeBoxTemplate
        {
            public override IdContainer GetId() => nameof(Start50);

            [DontOverwrite]
            public override LocalizationOption LoadLocalization()
            {
                throw new NotImplementedException();
            }

            public override JadeBoxConfig MakeConfig()
            {
                var config = JadeBoxConfig.FromId(GetId());
                config.Value1 = 200;
                return config;
            }
        }


        public static void GenJadeBoxes()
        {
            EntityManager.AddPostLoadAction(() =>
            {
                jadeBoxGen.QueueGen(nameof(SlowMana), overwriteVanilla: true, loadLocalization: () => new DirectLocalization(new Dictionary<string, object>() { { "Name", "Deeznuts"}}, mergeTerms:true));


                jadeBoxGen.FinalizeGen();

            });
        }

    }
}
