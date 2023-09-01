using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core.Cards;
using LBoL.EntityLib.Cards.Neutral.Black;
using LBoL.EntityLib.Cards.Neutral.Red;
using LBoL.EntityLib.Cards.Other.Adventure;
using LBoL.EntityLib.Exhibits.Common;
using LBoLEntitySideloader;
using LBoLEntitySideloader.Attributes;
using LBoLEntitySideloader.Entities;
using LBoLEntitySideloader.Resource;
using System;
using System.Collections.Generic;
using System.Text;
using static GenTests2.BepinexPlugin;

namespace GenTests2
{

    internal class Generation
    {

        internal static void InitTemplateGen()
        {


            EntityManager.AddPostLoadAction(() =>
            {


                Func<CardConfig> cardConfig = () => (new DummyCardTemplate()).DefaultConfig();
                Func<CardImages> cardImages = () => null;
                Func<LocalizationOption> cardLoc = () => new GlobalLocalization(embeddedSource, true);

                cardGen.QueueGen(nameof(YukariAttack), overwriteVanilla: true, loadLocalization: () =>
                {
                    var gl = new GlobalLocalization(embeddedSource, mergeTerms: true);
                    gl.LocalizationFiles.AddLocaleFile(LBoL.Core.Locale.En, "CardsEn");

                    return gl;
                });
                cardGen.QueueGen(nameof(HuaBlock), overwriteVanilla: true, loadLocalization: cardLoc);



                cardGen.QueueGen("NewGenCard", overwriteVanilla: false, cardConfig, null, null, generateEmptyLogic: true);


                exhibitGen.QueueGen(nameof(Yaoshi), overwriteVanilla: true, makeConfig: () =>
                {

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
                        Rarity: Rarity.Rare,
                        Value1: null,
                        Value2: null,
                        Value3: null,
                        Mana: new ManaGroup() { Philosophy = 3 },
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

                exhibitGen.OutputCSharpCode(outputToFile: true); // for debug

                exhibitGen.FinalizeGen();
                cardGen.FinalizeGen();
            });
        }


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