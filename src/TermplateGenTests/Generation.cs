using LBoL.ConfigData;
using LBoL.EntityLib.Cards.Neutral.Red;
using LBoL.EntityLib.Cards.Other.Adventure;
using LBoLEntitySideloader;
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

        internal static void QueueGen()
        {
            EntityManager.AddPostLoadAction(() => {

                var cardGen = new CardGen();

                Func<CardConfig> cardConfig = () => (new DummyCardTemplate()).DefaultConfig();
                Func<CardImages> cardImages = () => throw new NotImplementedException();
                Func<LocalizationOption> cardLoc = () => new GlobalLocalization(embeddedSource, true);

                cardGen.QueueGen(nameof(SuikaBigball), overwriteVanilla: true, loadLocalization: () => {
                    var gl = new GlobalLocalization(embeddedSource, mergeTerms: true);
                    gl.LocalizationFiles.AddLocaleFile(LBoL.Core.Locale.En, "CardsEn");

                    return gl;
                });
                cardGen.QueueGen(nameof(MeihongAttack), overwriteVanilla: true, loadLocalization: cardLoc);


                cardGen.OutputCSharpCode(outputToFile: true); // for debug
                cardGen.FinalizeGen();
            });
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
