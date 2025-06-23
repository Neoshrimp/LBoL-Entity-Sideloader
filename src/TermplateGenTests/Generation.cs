using HarmonyLib;
using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Cards;
using LBoL.EntityLib.Cards.Adventure;
using LBoL.EntityLib.Cards.Neutral.Red;
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


            // all template gen relate stuff goes to this action
            EntityManager.AddPostLoadAction(() => {

                // binds concrete entity logic to newly generated template definitions. should be add to PostLoadAction for hot reload to work.
                // hot reload is a bit unstable with dynamic gen
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



                // QuqueGen method takes various Func<T> arguments
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


                // actual practical application
                // change all appearances of |Flawless| to |Hit me, bitch|
                // also this only works if game is set to English at startup
                foreach (var id in TypeFactory<Card>._typeLocalizers)
                {
                    foreach (var term in TypeFactory<Card>._typeLocalizers[id.Key])
                    {
                        if (term.Key == "Name")
                            continue;

                        var newTermDic = new Dictionary<string, object>();


                        if (term.Value.ToString().Contains("|Flawless|"))
                        {
                            newTermDic.Add(term.Key, term.Value.ToString().Replace("|Flawless|", "|Hit me, bitch|"));
                        }

                        // or _typeLocalizers could have been modified fucking directly..
                        if (newTermDic.Count > 0)
                        { 
                            
                            cardGen.QueueGen(id.Key, overwriteVanilla: true, makeConfig: null, loadCardImages: null, loadLocalization: () => new DirectLocalization(newTermDic, mergeTerms:true));
                        }

                    }
                }




                cardGen.OutputCSharpCode(outputToFile: true); // for debug

                // FinalizeGen should only
                exhibitGen.FinalizeGen();
                cardGen.FinalizeGen();
            });
        }
        




        public sealed class BrandNewCardForTestDef : CardTemplate
        {
            public override IdContainer GetId()
            {
                return nameof(BrandNewCardForTest);
            }

            public override CardImages LoadCardImages()
            {
                return null;
            }

            public override LocalizationOption LoadLocalization()
            {
                return new DirectLocalization(new Dictionary<string, object>() { { "Name", "Deez" }, { "Description", "Deez" } });
            }

            public override CardConfig MakeConfig()
            {
                var config = DefaultConfig();
                config.Type = CardType.Defense;
                config.Colors = new List<ManaColor> { ManaColor.Red, ManaColor.White };
                config.Rarity = Rarity.Rare;
                return config;
            }

            [EntityLogic(typeof(BrandNewCardForTestDef))]
            public sealed class BrandNewCardForTest : Card
            { 
            
            }


        }

        [ExternalEntityLogic] // this attribute does nothing but suppresses warnings from Sideloader
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
