using HarmonyLib;
using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Cards;
using LBoL.EntityLib.Cards.Neutral.Red;
using LBoL.EntityLib.Cards.Other.Adventure;
using LBoL.EntityLib.Cards.Other.Misfortune;
using LBoL.EntityLib.Exhibits.Shining;
using LBoLEntitySideloader;
using LBoLEntitySideloader.Attributes;
using LBoLEntitySideloader.Entities;
using LBoLEntitySideloader.Resource;
using LBoLEntitySideloader.TemplateGen;
using System;
using System.Collections.Generic;
using System.Text;
using static TemplateGenTests.BepinexPlugin;

namespace TemplateGenTests
{
    internal class Generation
    {

        internal static void InitTemplateGen()
        {



            EntityManager.AddPostLoadAction(() => {

                // binds concrete entity logic to newly generated template definitions. should be add to PostLoadAction for hot reload to work.
                EntityManager.AddExternalDefinitionTypePromise(typeof(SomeNewCard), cardGen.GetDefTypePromise(nameof(SomeNewCard)));

                EntityManager.AddExternalDefinitionTypePromise(typeof(NewExhibit), exhibitGen.GetDefTypePromise(nameof(NewExhibit)));

                Func<CardConfig> cardConfig = () => (new DummyCardTemplate()).DefaultConfig();
                Func<CardImages> cardImages = () => null;
                Func<LocalizationOption> cardLoc = () => new GlobalLocalization(embeddedSource, true);

                cardGen.QueueGen(nameof(SuikaBigball), overwriteVanilla: true, loadLocalization: () => {
                    var gl = new GlobalLocalization(embeddedSource, mergeTerms: true);
                    gl.LocalizationFiles.AddLocaleFile(LBoL.Core.Locale.En, "CardsEn");

                    return gl;
                });
                cardGen.QueueGen(nameof(MeihongAttack), overwriteVanilla: true, loadLocalization: () => new DirectLocalization(new Dictionary<string, object>() { { "Name", "Phoenix Slap" } }, mergeTerms:true));


                cardGen.QueueGen(nameof(SomeNewCard), overwriteVanilla: false, makeConfig: () => {
                    var con = cardConfig();
                    con.Type = LBoL.Base.CardType.Attack;
                    con.Cost = new ManaGroup { Any = 0 };
                    con.Colors = new List<ManaColor>() { ManaColor.Red, ManaColor.White };
                    con.Rarity = Rarity.Uncommon;
                    con.IsPooled = true;
                    con.Damage = 10;
                    return con;
                },
                loadCardImages: null, 
                loadLocalization: cardLoc,
                generateEmptyLogic: false);




                exhibitGen.QueueGen(nameof(NewExhibit), overwriteVanilla: false, makeConfig: () => {

                    var exhibitConfig = new ExhibitConfig(
                        Index: 0,
                        Id: "",
                        Order: 10,
                        IsDebug: false,
                        IsPooled: true,
                        IsSentinel: false,
                        Revealable: false,
                        Appearance: AppearanceType.NonShop,
                        Owner: "",
                        LosableType: ExhibitLosableType.Losable,
                        Rarity: Rarity.Mythic,
                        Value1: null,
                        Value2: null,
                        Value3: null,
                        Mana: null,
                        BaseManaRequirement: null,
                        BaseManaColor: null,
                        BaseManaAmount: 0,
                        HasCounter: false,
                        InitialCounter: null,
                        Keywords: Keyword.None,
                        RelativeEffects: new List<string>() { },
                        RelativeCards: new List<string>() { }
                    );

                    return exhibitConfig;

                }, null, null);

                cardGen.OutputCSharpCode(outputToFile: true); // for debug


                exhibitGen.FinalizeGen();
                cardGen.FinalizeGen();
            });
        }

        [ExternalEntityLogic]
        public sealed class NewExhibit : Exhibit
        {
        }

        [ExternalEntityLogic]
        public sealed class SomeNewCard : Card
        {
            public override void Initialize()
            {
                base.Initialize();
                log.LogDebug("DEEEEEZ NUTS");
            }

            protected override IEnumerable<BattleAction> Actions(UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
            {
    			yield return base.AttackAction(selector);
            }


        }



        // since type not public sealed it's not gonna be picked up by SideLoader
        class DummyCardTemplate : CardTemplate
        {
            public override IdContainer GetId()
            {
                throw new NotImplementedException();
            }

            public override CardImages LoadCardImages()
            {
                throw new NotImplementedException();
            }

            public override LocalizationOption LoadLocalization()
            {
                throw new NotImplementedException();

            }

            public override CardConfig MakeConfig()
            {
                throw new NotImplementedException();
            }
        }




    }
}
