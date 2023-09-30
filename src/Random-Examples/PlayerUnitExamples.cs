using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using DG.Tweening;
using HarmonyLib;
using LBoL.Base;
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
        public static DirectorySource dir = new DirectorySource(PluginInfo.GUID, "Suika");

        public static string name = nameof(Suika);

        public override IdContainer GetId() => nameof(Suika);

        public override LocalizationOption LoadLocalization()
        {
            var gl = new GlobalLocalization(embeddedSource);
            gl.LocalizationFiles.AddLocaleFile(Locale.En, "PlayerUnitEn");
            return gl;
        }

        public override PlayerImages LoadPlayerImages()
        {
            var sprites = new PlayerImages();

            sprites.AutoLoad("", (s) => ResourceLoader.LoadSprite(s, dir, ppu: 100, 1, FilterMode.Bilinear, generateMipMaps: true), (s) => ResourceLoader.LoadSpriteAsync(s, dir));


            return sprites;
        
        }
            
        

        public override PlayerUnitConfig MakeConfig()
        {
            var reimuConfig = PlayerUnitConfig.FromId("Reimu").Copy();

            var config = new PlayerUnitConfig(
            Id: "",
            ShowOrder: 6,
            Order: 0,
            UnlockLevel: 0,
            ModleName: "",
            NarrativeColor: "#e58c27",
            IsSelectable: true,
            MaxHp: 90,
            InitialMana: new LBoL.Base.ManaGroup() { Red = 2, Blue = 1, White = 1 },
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


    public sealed class GoblinPunchCardDef : CardTemplate
    {
        public override IdContainer GetId() => nameof(GoblinPunch);



        public override CardImages LoadCardImages() => new CardImages(embeddedSource, ResourceLoader.LoadTexture("goblinPunch.png", embeddedSource));



        public override LocalizationOption LoadLocalization() => new GlobalLocalization(embeddedSource);



        public override CardConfig MakeConfig()
        {
            var config = DefaultConfig();

            config.GunName = "Simple1";
            config.Colors = new List<ManaColor>() { ManaColor.Red };
            config.Cost = new ManaGroup() { Any = 1, Red = 1 };

            config.Owner = "Suika";

            config.Type = CardType.Attack;
            config.TargetType = TargetType.SingleEnemy;

            config.Damage = 13;
            config.UpgradedDamage = 16;

            config.Illustrator = "MTB";

            return config;
        }


        [EntityLogic(typeof(GoblinPunchCardDef))]
        public sealed class GoblinPunch : Card
        { 
        
        }


    }



    public sealed class GoblinPunchRareCardDef : CardTemplate
    {
        public override IdContainer GetId() => nameof(GoblinPunchRare);



        public override CardImages LoadCardImages() => new CardImages(embeddedSource, ResourceLoader.LoadTexture("goblinPunch.png", embeddedSource));



        public override LocalizationOption LoadLocalization() => new GlobalLocalization(embeddedSource);



        public override CardConfig MakeConfig()
        {
            var config = new GoblinPunchCardDef().MakeConfig();
            config.Rarity = Rarity.Rare;
            config.Damage = 14;
            config.UpgradedDamage = 17;
            return config;
        }


        [EntityLogic(typeof(GoblinPunchRareCardDef))]
        public sealed class GoblinPunchRare : Card
        {

        }


    }


    public sealed class BeaPlayerDef : PlayerUnitTemplate
    {


        public override IdContainer GetId() => nameof(Bea);

        public override LocalizationOption LoadLocalization() => new GlobalLocalization(embeddedSource);

        public override PlayerImages LoadPlayerImages()
        {
            var sprites = new PlayerImages();

            // 2do for some reason this method works better for Bea..
            var asyncLoading = ResourceLoader.LoadSpriteAsync("bea.png", directorySource);

            sprites.SetStartPanelStand(asyncLoading);
            sprites.SetWinStand(asyncLoading);
            sprites.SetDeckStand(asyncLoading);

            return sprites;
        }


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


        public override PlayerImages LoadPlayerImages()
        {
            var sprites = new PlayerImages();

            // 2do for some reason this method works better for Keiki
            var loading = ResourceLoader.LoadSprite("keiki.png", directorySource);

            sprites.SetStartPanelStand(null, () => loading);
            sprites.SetWinStand(null, () => loading);
            sprites.SetDeckStand(null, () => loading);

            return sprites;
        }


        public override PlayerUnitConfig MakeConfig()
        {
            var config = PlayerUnitConfig.FromId("Reimu").Copy();
            return config;
        }

        [EntityLogic(typeof(KeikiPlayerDef))]
        public sealed class Keiki : PlayerUnit { }

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

            return new ModelOption(ResourceLoader.LoadSpriteAsync("Youmu.png", directorySource, ppu: 565));

            return new ModelOption(ResourcesHelper.LoadSpineUnitAsync("Remilia"));
        }


        public override UniTask<Sprite> LoadSpellSprite() => ResourceLoader.LoadSpriteAsync("Stand.png", SuikaPlayerDef.dir, ppu: 1200);


        public override UnitModelConfig MakeConfig()
        {
/*            var config = UnitModelConfig.FromName("Remilia").Copy();
            config.Flip = true;
            return config;
 */

            var config = UnitModelConfig.FromName("Youmu").Copy();
            config.Flip = false;
            config.Type = 0;
            config.Offset = new Vector2(0, 0.04f);
            return config;

        }
    }


    public sealed class BeaModelDef : UnitModelTemplate
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


        public override UniTask<Sprite> LoadSpellSprite() => ResourceLoader.LoadSpriteAsync("bea.png", directorySource, ppu: 600);

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

        public override LocalizationOption LoadLocalization() => new DirectLocalization(new Dictionary<string, object>() { { "Default", "Keiki Haniyasushin" }, { "Short", "Keiki" } });

        public override ModelOption LoadModelOptions()
        {
            return new ModelOption(ResourcesHelper.LoadSpineUnitAsync("Remilia"));
        }


        public override UniTask<Sprite> LoadSpellSprite() => ResourceLoader.LoadSpriteAsync("keiki.png", directorySource, ppu: 600);


        public override UnitModelConfig MakeConfig()
        {
            var config = UnitModelConfig.FromName("Remilia").Copy();
            config.Flip = true;
            return config;
        }
    }








}
