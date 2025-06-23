using HarmonyLib;
using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Cards;
using LBoL.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Reflection;
using LBoL.Presentation.UI.Panels;
using LBoLEntitySideloader.ReflectionHelpers;
using System.Collections;
using LBoL.Core.Battle;
using LBoL.EntityLib.Exhibits.Shining;
using LBoL.EntityLib.EnemyUnits.Character;
using LBoL.Presentation.UI.ExtraWidgets;
using LBoL.Presentation;
using LBoL.EntityLib.Adventures;
using LBoL.Base.Extensions;
using LBoL.Core.StatusEffects;
using JetBrains.Annotations;
using Mono.CSharp;
using LBoL.Core.Stations;

namespace LBoLEntitySideloader.GameFixes
{

    [HarmonyPatch(typeof(ShopStation), "OnEnter")]
    class ShopStation_Patch
    {
        static int CheckDiscountIndex(ShopStation shopStation, int dIndex, List<ShopItem<Card>> cards)
        {
            if (dIndex >= 0 && dIndex < cards.Count)
                return dIndex;

            var gr = shopStation.GameRun;

            int newIndex = gr.ShopRng.NextInt(0, cards.Count);

            shopStation.DiscountCardNo = newIndex;

            return newIndex;

            
        }

        private static ShopItem<Card> IndexGuard(List<ShopItem<Card>> shopCards, int index)
        {
            return shopCards.TryGetValue(index) ?? new ShopItem<Card>(null, null, 420);
        }


        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            return new CodeMatcher(instructions)
                 .MatchEndForward(new CodeMatch(ci => ci.opcode == OpCodes.Callvirt && ci.operand is MethodBase mb && mb.Name == "get_Item"))
                 .MatchEndBackwards(new CodeMatch(OpCodes.Ldfld))
                 .InsertAndAdvance(new CodeInstruction(OpCodes.Ldarg_0))
                 .Advance(1)

                 .InsertAndAdvance(new CodeInstruction(OpCodes.Ldloc_0))
                 .InsertAndAdvance(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ShopStation_Patch), nameof(ShopStation_Patch.CheckDiscountIndex))))

                 // in case there are no cards at all
                 .MatchEndForward(new CodeMatch(ci => ci.opcode == OpCodes.Callvirt && ci.operand is MethodBase mb && mb.Name == "get_Item"))
                 .SetInstruction(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ShopStation_Patch), nameof(ShopStation_Patch.IndexGuard))))

                 .InstructionEnumeration();
        }


    }


    [HarmonyPatch(typeof(ShopPanel), nameof(ShopPanel.SetShop))]
    class ShopPanel_Patch
    {


        private static ShopItem<Card> CardIndexGuard(List<ShopItem<Card>> shopCards, int index)
        {
            return shopCards.TryGetValue(index);
        }

        private static ShopItem<Exhibit> ExhibitIndexGuard(List<ShopItem<Exhibit>> shopExhibits, int index)
        {
            return shopExhibits.TryGetValue(index);
        }


        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return new CodeMatcher(instructions)
                 .MatchEndForward(new CodeMatch(ci => ci.opcode == OpCodes.Callvirt && ci.operand is MethodBase mb && mb.Name == "get_Item"))
                 .SetInstruction(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ShopPanel_Patch), nameof(ShopPanel_Patch.CardIndexGuard))))

                 .MatchEndForward(new CodeMatch(ci => ci.opcode == OpCodes.Ldstr && ci.operand.ToString().Contains("shopExhibits")))
                 .ThrowIfInvalid("shopExhibits is no longer a magic string")

                 .MatchEndForward(new CodeMatch(ci => ci.opcode == OpCodes.Callvirt && ci.operand is MethodBase mb && mb.Name == "get_Item"))
                 .SetInstruction(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ShopPanel_Patch), nameof(ShopPanel_Patch.ExhibitIndexGuard))))


                 .InstructionEnumeration();
        }

        
    }



}
