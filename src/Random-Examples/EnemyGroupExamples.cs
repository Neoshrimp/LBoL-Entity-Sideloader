using HarmonyLib;
using LBoL.Base;
using LBoL.ConfigData;
using LBoL.EntityLib.EnemyUnits.Character;
using LBoL.EntityLib.EnemyUnits.Normal;
using LBoL.EntityLib.EnemyUnits.Normal.Ravens;
using LBoLEntitySideloader;
using LBoLEntitySideloader.Attributes;
using LBoLEntitySideloader.Entities;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;
using UnityEngine;
using static Random_Examples.BepinexPlugin;

namespace Random_Examples
{

    static public class CustomFormations
    {

        public const string VedgeSmall = "VedgeSmall";

        public const string Vedge = "Vedge";

        public const string Surrounded = "Surrounded";



        static internal void AddFormations()
        {
            EnemyGroupTemplate.AddFormation(VedgeSmall, new Dictionary<int, Vector2>() {
                { 0, new Vector2(0, 0) },
                { 1, new Vector2(2, 2) },
                { 2, new Vector2(2, -2) },
            });


            EnemyGroupTemplate.AddFormation(Vedge, new Dictionary<int, Vector2>() {
                { 0, new Vector2(1.2f, 0+0.5f) },
                { 1, new Vector2(3f, 1.5f+0.5f) },
                { 2, new Vector2(3f, -1.5f+0.5f) },
                { 3, new Vector2(5f, 1.8f+0.5f) },
                { 4, new Vector2(5f, -1.8f+0.5f) },
            });

            EnemyGroupTemplate.AddFormation(Surrounded, new Dictionary<int, Vector2>() {
                { 0, new Vector2(3, 0) },
                { 1, new Vector2(-3, 2) },
            });


        }
    }

    [OverwriteVanilla]
    public sealed class TenguHeavenDef : EnemyGroupTemplate
    {
        // 3 fairies
        public override IdContainer GetId() => "Sanyue";


        public override EnemyGroupConfig MakeConfig()
        {
            var config = new EnemyGroupConfig(
                Id: "",
                Name: "TenguHeaven",
                FormationName: VanillaFormations.Diamond,
                // because of the way 3 fairies are coded they really need to stick together <3
                // needs a slight code fix for Sunny to work but it's included in the Sideloader
                Enemies: new List<string>() { nameof(Aya), "Sunny", "Luna", "Star" },
                EnemyType: EnemyType.Elite,
                DebutTime: 1f,
                RollBossExhibit: false,
                PlayerRoot: new Vector2(-4f, 0.5f),
                PreBattleDialogName: "",
                PostBattleDialogName: ""
            );
            return config;
        }
    }


    [OverwriteVanilla]
    public sealed class BallsAndGirlDef : EnemyGroupTemplate
    {
        public override IdContainer GetId() => "32";


        public override EnemyGroupConfig MakeConfig()
        {
            var config = new EnemyGroupConfig(
                Id: "",
                Name: "BallsAndGirl",
                FormationName: CustomFormations.Vedge,
                Enemies: new List<string>() { "SickGirl", "YinyangyuBlue", "YinyangyuBlue", "YinyangyuRed", "YinyangyuRed" },
                EnemyType: EnemyType.Normal,
                DebutTime: 1f,
                RollBossExhibit: false,
                PlayerRoot: new Vector2(-4f, 0.5f),
                PreBattleDialogName: "",
                PostBattleDialogName: ""
            );
            return config;
        }
    }


    public sealed class ThreeCrowsGroupDef : EnemyGroupTemplate
    {
        public override IdContainer GetId() => "ThreeCrows";

        public override EnemyGroupConfig MakeConfig()
        {
            var config = new EnemyGroupConfig(
                Id: "",
                Name: "ThreeCrows",
                FormationName: VanillaFormations.Triangle,
                Enemies: new List<string>() { nameof(RavenWen), nameof(RavenGuo), nameof(RavenGuo) },
                EnemyType: EnemyType.Normal,
                DebutTime: 1f,
                RollBossExhibit: false,
                PlayerRoot: new Vector2(-4f, 0.5f),
                PreBattleDialogName: "",
                PostBattleDialogName: ""
            );
            return config;
        }


    }



}
