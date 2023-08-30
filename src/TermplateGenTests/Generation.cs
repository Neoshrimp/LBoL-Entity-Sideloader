using HarmonyLib;
using LBoL.ConfigData;
using LBoL.Core.Cards;
using LBoL.EntityLib.Cards.Neutral.Red;
using LBoL.EntityLib.Cards.Other.Adventure;
using LBoL.EntityLib.Cards.Other.Misfortune;
using LBoLEntitySideloader;
using LBoLEntitySideloader.Attributes;
using LBoLEntitySideloader.Entities;
using LBoLEntitySideloader.Resource;
using LBoLEntitySideloader.TemplateGen;
using System;
using System.Collections.Generic;
using System.Text;
using static TermplateGenTests.BepinexPlugin;

namespace TermplateGenTests
{
    internal class Generation
    {

        internal static void InitTemplateGen()
        {

            EntityManager.AddExternalDefinitionTypePromise(typeof(SomeNewCard), cardGen.GetDefTypePromise(nameof(SomeNewCard)));

            EntityManager.AddPostLoadAction(() => {


                Func<CardConfig> cardConfig = () => (new DummyCardTemplate()).DefaultConfig();
                Func<CardImages> cardImages = () => null;
                Func<LocalizationOption> cardLoc = () => new GlobalLocalization(embeddedSource, true);

                cardGen.QueueGen(nameof(SuikaBigball), overwriteVanilla: true, loadLocalization: () => {
                    var gl = new GlobalLocalization(embeddedSource, mergeTerms: true);
                    gl.LocalizationFiles.AddLocaleFile(LBoL.Core.Locale.En, "CardsEn");

                    return gl;
                });
                cardGen.QueueGen(nameof(MeihongAttack), overwriteVanilla: true, loadLocalization: cardLoc);


                cardGen.QueueGen(nameof(SomeNewCard), overwriteVanilla: false, makeConfig: () => {
                    var con = cardConfig();
                    con.Type = LBoL.Base.CardType.Misfortune;
                    return con;
                },
                loadCardImages: null, 
                loadLocalization: cardLoc,
                generateEmptyLogic: false);

                cardGen.OutputCSharpCode(outputToFile: true); // for debug

                cardGen.FinalizeGen();
            });
        }


        [ExternalEntityLogic]
        public sealed class SomeNewCard : Card
        {
            public override void Initialize()
            {
                base.Initialize();
                log.LogDebug("DEEEEEZ NUTS");
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
