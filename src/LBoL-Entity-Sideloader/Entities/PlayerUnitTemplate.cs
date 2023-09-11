using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Units;
using System;
using System.Collections.Generic;
using System.Text;
using static UnityEngine.UI.GridLayoutGroup;
using UnityEngine.UI;
using UnityEngine;
using LBoL.Presentation.UI.Panels;
using HarmonyLib;
using System.Reflection.Emit;
using LBoL.EntityLib.Exhibits.Shining;
using LBoL.Core.Cards;
using Mono.CSharp;

namespace LBoLEntitySideloader.Entities
{
    // soon(tm)
    // 2do test
    public abstract class PlayerUnitTemplate : EntityDefinition,
        IConfigProvider<PlayerUnitConfig>,
        IGameEntityProvider<PlayerUnit>
    {
        public override Type ConfigType() => typeof(PlayerUnitConfig);

        public override Type EntityType() => typeof(PlayerUnit);

        public override Type TemplateType() => typeof(PlayerUnitTemplate);




        // 2do patch hardcoded 5 players
/*        [HarmonyPatch(typeof(StartGamePanel), nameof(StartGamePanel.SelectPlayer))]
        class SelectPlayer_Patch
        {

            static void ExpandVariants(StartGamePanel startGamePanel)
            {
                var config = startGamePanel._player.Config;
                if(config.Id == CardTemplate.VanillaCharNames.Reimu)
                    startGamePanel._typeCandidates.AddItem(new StartGamePanel.TypeCandidate() {
                        Name = "TypeC",
                        Us = LBoL.Core.Library.CreateUs(typeof()),
                        Exhibit = LBoL.Core.Library.CreateExhibit(typeof(CirnoU)),
                        Deck = config.DeckB.Select(new Func<string, Card>(LBoL.Core.Library.CreateCard)).ToArray<Card>()
                    });
            }

            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {

                return new CodeMatcher(instructions)
                    .End()
                    .MatchBack(false, new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(StartGamePanel), nameof(StartGamePanel.SelectType))))
                    .Advance(-1)
                    .Insert(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(SelectPlayer_Patch), nameof(SelectPlayer_Patch.ExpandVariants))))
                    .Advance(-1)
                    .Insert(new CodeInstruction(OpCodes.Ldarg_0))
                    .InstructionEnumeration();

            }

        }*/




        /// <summary> 
        /// Id : 
        /// ShowOrder : show order in game start panel (defacto index)
        /// Order : ordering priority for character's cards in collection // 2do make unique like index?
        /// UnlockLevel : should be 0 to make character available right away
        /// ModleName : always ""
        /// NarrativeColor : color hex code
        /// IsSelectable : show character filter in collection??
        /// MaxHp : 
        /// InitialMana : 
        /// InitialMoney : 
        /// InitialPower : 
        /// UltimateSkillA : 
        /// UltimateSkillB : 
        /// ExhibitA : 
        /// ExhibitB : 
        /// DeckA : 
        /// DeckB : 
        /// DifficultyA : number from 1 to 3
        /// DifficultyB : number from 1 to 3
        /// </summary>
        /// <returns></returns>
        public PlayerUnitConfig DefaultConfig()
        {
            var config = new PlayerUnitConfig(
                    Id : "",
                    ShowOrder : 0,
                    Order : 0,
                    UnlockLevel : 0,
                    ModleName : "",
                    NarrativeColor : "",
                    IsSelectable : true,
                    MaxHp : 1,
                    InitialMana : new LBoL.Base.ManaGroup() { },
                    InitialMoney : 1,
                    InitialPower : 0,
                    UltimateSkillA : "",
                    UltimateSkillB : "",
                    ExhibitA : "",
                    ExhibitB : "",
                    DeckA : new List<string>() { },
                    DeckB : new List<string>() { },
                    DifficultyA : 1,
                    DifficultyB : 1
                
                );
            return config;
        }


        public abstract PlayerUnitConfig MakeConfig();

    }
}
