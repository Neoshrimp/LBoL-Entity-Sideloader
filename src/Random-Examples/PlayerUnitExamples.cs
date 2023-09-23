using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using DG.Tweening;
using HarmonyLib;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Cards;
using LBoL.Core.Randoms;
using LBoL.Core.Units;
using LBoL.EntityLib.Cards.Character.Reimu;
using LBoL.EntityLib.Cards.Neutral.NoColor;
using LBoL.EntityLib.Exhibits.Common;
using LBoL.EntityLib.Exhibits.Shining;
using LBoL.EntityLib.PlayerUnits;
using LBoL.Presentation;
using LBoL.Presentation.UI;
using LBoL.Presentation.UI.Panels;
using LBoL.Presentation.UI.Widgets;
using LBoLEntitySideloader;
using LBoLEntitySideloader.Attributes;
using LBoLEntitySideloader.Entities;
using LBoLEntitySideloader.Resource;
using LBoLEntitySideloader.UIhelpers;
using LBoLEntitySideloader.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngine.Windows;
using static Random_Examples.BepinexPlugin;


namespace Random_Examples
{




    public sealed class SuikaPlayerDef : PlayerUnitTemplate
    {
        static DirectorySource dir = new DirectorySource(PluginInfo.GUID, "Suika");

        public static string name = nameof(Suika);

        public override IdContainer GetId() => nameof(Suika);

        public override LocalizationOption LoadLocalization()
        {
            var gl = new GlobalLocalization(embeddedSource);
            gl.LocalizationFiles.AddLocaleFile(Locale.En, "PlayerUnitEn");
            return gl;
        }

        public override PlayerImages LoadPlayerSprites()
        {
            var sprites = new PlayerImages();
            sprites.startPanelStandPic = () => ResourceLoader.LoadSprite("Suika.png", dir, ppu: 1200, anisoLevel: 16, filterMode: FilterMode.Trilinear);

            sprites.inRunAvatarPic = ResourceLoader.LoadSprite("SuikaAvatar.png", dir, ppu: 400, anisoLevel: 16, filterMode: FilterMode.Trilinear);

            sprites.collectionIcon = ResourceLoader.LoadSprite("SuikaAvatar.png", dir, ppu: 400, anisoLevel: 16, filterMode: FilterMode.Trilinear);

            sprites.selectionCircleIcon = ResourceLoader.LoadSprite("SuikaAvatar.png", dir, ppu: 400, anisoLevel: 16, filterMode: FilterMode.Trilinear);


            return sprites;
        
        }
            
        

        public override PlayerUnitConfig MakeConfig()
        {
            var reimuConfig = PlayerUnitConfig.FromId("Reimu").Copy();

            var config = new PlayerUnitConfig(
            Id: "",
            ShowOrder: 0,
            Order: 0,
            UnlockLevel: 0,
            ModleName: "",
            NarrativeColor: "#f241a8",
            IsSelectable: true,
            MaxHp: 90,
            InitialMana: new LBoL.Base.ManaGroup() { Red = 2, Green = 1, White = 1 },
            InitialMoney: 3,
            InitialPower: 30,
            //temp
            UltimateSkillA: reimuConfig.UltimateSkillA,
            UltimateSkillB: reimuConfig.UltimateSkillB,
            ExhibitA: reimuConfig.ExhibitA,
            ExhibitB: reimuConfig.ExhibitB,
            DeckA: reimuConfig.DeckA,
            DeckB: reimuConfig.DeckB,
            DifficultyA: 1,
            DifficultyB: 1
            );
            return config;
        }


        [EntityLogic(typeof(SuikaPlayerDef))]
        public sealed class Suika : PlayerUnit { }

    }



/*    public sealed class BeaPlayerDef : PlayerUnitTemplate
    {


        public override IdContainer GetId() => nameof(Bea);

        public override LocalizationOption LoadLocalization() => new GlobalLocalization(embeddedSource);

        public override Sprite LoadStandSprite() => ResourceLoader.LoadSprite("bea.png", directorySource, ppu: 800, anisoLevel: 16, filterMode: FilterMode.Trilinear);


        public override PlayerUnitConfig MakeConfig()
        {
            var config = PlayerUnitConfig.FromId("Reimu").Copy();
            return config;
        }

        [EntityLogic(typeof(BeaPlayerDef))]
        public sealed class Bea : PlayerUnit { }

    }



    public sealed class KeikiPlayerDef : PlayerUnitTemplate
    {
        public override IdContainer GetId() => nameof(Keiki);

        public override LocalizationOption LoadLocalization() => new GlobalLocalization(embeddedSource);


        public override Sprite LoadStandSprite() => ResourceLoader.LoadSprite("keiki.png", directorySource, ppu: 700, anisoLevel: 16, filterMode: FilterMode.Trilinear);


        public override PlayerUnitConfig MakeConfig()
        {
            var config = PlayerUnitConfig.FromId("Reimu").Copy();
            return config;
        }

        [EntityLogic(typeof(KeikiPlayerDef))]
        public sealed class Keiki : PlayerUnit { }

    }*/

    // 2do make generic
    [HarmonyPatch(typeof(MuseumPanel), nameof(MuseumPanel.Awake))]
    class MuseumPanel_Patch
    {
        static void Prefix(MuseumPanel __instance)
        {


/*
            __instance.portraitList.TryAdd("Bea", __instance.portraitList["Reimu"]);
            __instance.portraitList.TryAdd("Keiki", __instance.portraitList["Reimu"]);*/

        }

    }


    [HarmonyPatch(typeof(StartGamePanel), nameof(StartGamePanel.Awake))]
    class StartGamePanel_Patch
    {
        static void Prefix(StartGamePanel __instance)
        {


/*            var sprite1 = ResourceLoader.LoadSprite("bea.png", directorySource, ppu: 800, anisoLevel: 16, filterMode: FilterMode.Trilinear);
            __instance.standPicList.TryAdd("Bea", sprite1);
            __instance.headPicList.TryAdd("Bea", sprite1);

            var sprite2 = ResourceLoader.LoadSprite("keiki.png", directorySource, ppu: 700, anisoLevel: 16, filterMode: FilterMode.Trilinear);
            __instance.standPicList.TryAdd("Keiki", sprite2);
            __instance.headPicList.TryAdd("Keiki", sprite2);*/


        }

    }



    public sealed class SuikaModelDef : UnitModelTemplate
    {

        
        public override IdContainer GetId() => new SuikaPlayerDef().UniqueId;

        public override LocalizationOption LoadLocalization()
        {
            var gl = new GlobalLocalization(embeddedSource);
            gl.LocalizationFiles.mergeTerms = true;
            gl.LocalizationFiles.AddLocaleFile(Locale.En, "UnitModelEn");
            return gl;
        }

        public override ModelOption LoadModelOptions()
        {
            return new ModelOption(ResourcesHelper.LoadSpineUnitAsync("Remilia"));
        }


        public override UniTask<Sprite> LoadSpellSprite() => ResourceLoader.LoadSpriteAsync("Suika.png", directorySource, ppu: 1200, anisoLevel: 16, filterMode: FilterMode.Trilinear);


        public override UnitModelConfig MakeConfig()
        {
            var config = UnitModelConfig.FromName("Remilia").Copy();
            config.Flip = true;
            return config;
        }
    }


/*    public sealed class BeaModelDef : UnitModelTemplate
    {


        public override IdContainer GetId() => new BeaPlayerDef().UniqueId;

        public override LocalizationOption LoadLocalization() 
        { 
            var lf = new LocalizationFiles(embeddedSource);
            lf.AddLocaleFile(Locale.En, "BeaModelEn");
            lf.mergeTerms = true;
            return lf;
        } 
        


        public override ModelOption LoadModelOptions()
        {
            return new ModelOption(ResourcesHelper.LoadSpineUnitAsync("Remilia"));
        }

        static internal UniTask<Sprite> LoadSuikaSprite() => ResourceLoader.LoadSpriteAsync("Suika.png", directorySource, ppu: 1200, anisoLevel: 16, filterMode: FilterMode.Trilinear);

        public override UniTask<Sprite> LoadSpellSprite()
        {
            return LoadSuikaSprite();
        }

        public override UnitModelConfig MakeConfig()
        {
            var config = UnitModelConfig.FromName("Remilia").Copy();
            config.Flip = true;
            return config;
        }
    }


    public sealed class KeikiModelDef : UnitModelTemplate
    {
        public override IdContainer GetId() => new KeikiPlayerDef().UniqueId;

        public override LocalizationOption LoadLocalization() => new DirectLocalization(new Dictionary<string, object>() { { "Default", "Keiki Nuts"}, { "Short", "Keiki" } });

        public override ModelOption LoadModelOptions()
        {
            return new ModelOption(ResourcesHelper.LoadSpineUnitAsync("Remilia"));
        }

        static internal UniTask<Sprite> LoadSuikaSprite() => ResourceLoader.LoadSpriteAsync("Suika.png", directorySource, ppu: 1200, anisoLevel: 16, filterMode: FilterMode.Trilinear);

        public override UniTask<Sprite> LoadSpellSprite()
        {
            return LoadSuikaSprite();
        }

        public override UnitModelConfig MakeConfig()
        {
            var config = UnitModelConfig.FromName("Remilia").Copy();
            config.Flip = true;
            return config;
        }
    }*/




  


  
}
