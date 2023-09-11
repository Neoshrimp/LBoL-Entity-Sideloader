using HarmonyLib;
using LBoL.Base;
using LBoL.ConfigData;
using LBoL.EntityLib.EnemyUnits.Character;
using LBoL.EntityLib.EnemyUnits.Normal;
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


        [HarmonyPatch(typeof(Sunny), nameof(Sunny.OnEnterBattle))]
        class SunnyCastBug_Patch
        {

            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {

                foreach (var ci in instructions)
                {
                    if (ci.Is(OpCodes.Castclass, typeof(LightFairy)))
                    {
                        yield return new CodeInstruction(OpCodes.Isinst, typeof(LightFairy));
                    }
                    else
                    {
                        yield return ci;
                    }
                }
            }

        }


    }


    [OverwriteVanilla]
    public sealed class BallsAndGirlDef : EnemyGroupTemplate
    {
        // 3 fairies
        public override IdContainer GetId() => "32";


        public override EnemyGroupConfig MakeConfig()
        {
            var config = new EnemyGroupConfig(
                Id: "",
                Name: "BallsAndGirl",
                FormationName: CustomFormations.Vedge,
                // because of the way 3 fairies are coded they really need to stick together <3
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



}
