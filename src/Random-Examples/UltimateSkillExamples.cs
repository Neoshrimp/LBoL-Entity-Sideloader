using HarmonyLib;
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
using LBoL.EntityLib.UltimateSkills;
using LBoL.Presentation.UI.Panels;
using LBoLEntitySideloader;
using LBoLEntitySideloader.Attributes;
using LBoLEntitySideloader.Entities;
using LBoLEntitySideloader.ReflectionHelpers;
using LBoLEntitySideloader.Resource;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using static Random_Examples.BepinexPlugin;

namespace Random_Examples
{
    public sealed class ReimuUltRJabDef : UltimateSkillTemplate
    {
        public override IdContainer GetId() => nameof(ReimuUltJab);

        public override LocalizationOption LoadLocalization()
        {
            var gl = new GlobalLocalization(embeddedSource);
            gl.LocalizationFiles.AddLocaleFile(Locale.En, "UltimateSkill");
            return gl;
        }

        public override Sprite LoadSprite()
        {
            return ResourceLoader.LoadSprite("reimu_fist.png", embeddedSource);
        }   

        public override UltimateSkillConfig MakeConfig()
        {
            var config = new UltimateSkillConfig(
                Id: "",
                Order: 10,
                PowerCost: 45,
                PowerPerLevel: 100,
                MaxPowerLevel: 2,
                RepeatableType: UsRepeatableType.FreeToUse,
                Damage: 13,
                Value1: 0,
                Value2: 0,
                Keywords: Keyword.None,
                RelativeEffects: new List<string>() { },
                RelativeCards: new List<string>() { }
                );

            return config;
        }
    }

    [EntityLogic(typeof(ReimuUltRJabDef))]
    public sealed class ReimuUltJab : UltimateSkill
    {   
        public ReimuUltJab()
        {
            base.TargetType = TargetType.SingleEnemy;
            base.GunName = "博丽一拳";
        }

        // remove accurate
        public override DamageInfo Damage => DamageInfo.Attack(Config.Damage);


        protected override IEnumerable<BattleAction> Actions(UnitSelector selector)
        {
            yield return PerformAction.Spell(Owner, new ReimuUltRJabDef().UniqueId);

			yield return new DamageAction(base.Owner, selector.GetEnemy(base.Battle), this.Damage, base.GunName, GunType.Single);

        }
    }

    // not need. for testing only
/*    public sealed class ReimuUltJabSpellDef2: SpellTemplate
    {
        public override IdContainer GetId() => new ReimuUltRJabDef2().UniqueId;

        public override LocalizationOption LoadLocalization()
        {
            var gl = new GlobalLocalization(embeddedSource);
            gl.LocalizationFiles.AddLocaleFile(Locale.En, "SpellEn");
            return gl;
            //var lf = new LocalizationFiles(embeddedSource);
            //lf.AddLocaleFile(Locale.En, "SpellEn");
            //return lf;
        }
        //=> new DirectLocalization(new Dictionary<string, object> { { OnCastTitle, "deez" }, { OnCastName, "nuts" } });

        public override SpellConfig MakeConfig()
        {
            return DefaultConfig();
        }
    }


    public sealed class ReimuUltRJabDef2 : UltimateSkillTemplate
    {
        public override IdContainer GetId() => nameof(ReimuUltJab2);

        public override LocalizationOption LoadLocalization()
        {
            var gl = new GlobalLocalization(embeddedSource);
            gl.LocalizationFiles.AddLocaleFile(Locale.En, "UltimateSkill");
            return gl;
        }

        public override Sprite LoadSprite()
        {
            return ResourceLoader.LoadSprite("reimu_fist.png", embeddedSource);
        }

        public override UltimateSkillConfig MakeConfig()
        {
            var config = new UltimateSkillConfig(
                Id: "",
                Order: 10,
                PowerCost: 45,
                PowerPerLevel: 100,
                MaxPowerLevel: 2,
                RepeatableType: UsRepeatableType.FreeToUse,
                Damage: 13,
                Value1: 0,
                Value2: 0,
                Keywords: Keyword.None,
                RelativeEffects: new List<string>() { },
                RelativeCards: new List<string>() { }
                );

            return config;
        }
    }

    [EntityLogic(typeof(ReimuUltRJabDef2))]
    public sealed class ReimuUltJab2 : UltimateSkill
    {
        public ReimuUltJab2()
        {
            base.TargetType = TargetType.SingleEnemy;
            base.GunName = "博丽一拳";
        }

        // remove accurate
        public override DamageInfo Damage => DamageInfo.Attack(Config.Damage);


        protected override IEnumerable<BattleAction> Actions(UnitSelector selector)
        {
            yield return PerformAction.Spell(Owner, new ReimuUltRJabDef().UniqueId);

            yield return new DamageAction(base.Owner, selector.GetEnemy(base.Battle), this.Damage, base.GunName, GunType.Single);

        }
    }
*/

}
