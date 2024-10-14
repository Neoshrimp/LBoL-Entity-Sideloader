using System;
using System.Collections.Generic;
using System.Text;

namespace PatchWorkshop
{
    using HarmonyLib;
    using LBoL.Base;
    using LBoL.ConfigData;
    using LBoL.Core;
    using LBoL.Core.Randoms;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Text;
    using UnityEngine;

    namespace CyanBoosterPack.Utils
    {
        public interface ICustomColourIdentity
        {
            public IReadOnlyList<ManaColor> ColourIdentity { get; }
        }

        //[HarmonyPatch]
        //[HarmonyDebug]
        public sealed class CardIdentity
        {

            public static bool ManaMatchesCardIdentity(bool colorLimit, ManaGroup playerMana, CardConfig cardCfg)
            {
                IReadOnlyList<ManaColor> colourList = null;

                if ((object)cardCfg is ICustomColourIdentity id)
                {
                    colourList = id.ColourIdentity;
                }
                else
                {
                    colourList = cardCfg.Colors;
                }

                return !colorLimit || colourList.All(c => playerMana.GetValue(c) > 0);
            }

            public static MethodBase TargetMethod()
            {
                return AccessTools.Method(typeof(GameRunController), nameof(GameRunController.CreateValidCardsPool), new Type[] { typeof(CardWeightTable), typeof(ManaGroup?), typeof(bool), typeof(bool), typeof(bool), typeof(Predicate<CardConfig>) });
            }

            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
            {
/*                BepinexPlugin.log.LogDebug("Before:");
                foreach (var i in instructions)
                {
                    BepinexPlugin.log.LogDebug(i);
                }*/

                CodeMatcher cm = new CodeMatcher(instructions, generator)
                    .MatchStartForward(new CodeMatch[]
                    {
                    new CodeMatch(OpCodes.Ldarg_3),
                    new CodeMatch(OpCodes.Brfalse),
                    new CodeMatch(OpCodes.Ldloc_S),
                    new CodeMatch(OpCodes.Callvirt),
                    new CodeMatch(OpCodes.Ldloc_S),
                    new CodeMatch(OpCodes.Ldftn),
                    new CodeMatch(OpCodes.Newobj),
                    new CodeMatch(OpCodes.Call),
                    new CodeMatch(OpCodes.Brtrue)
                    }).ThrowIfNotMatch("Couldn't match this stupid colorLimit check");

                var labels = cm.Labels;
                var tgtLabel = cm.Advance(8).Operand;
                cm.Advance(-8);



                var myLabel = generator.DefineLabel();
                var l_manaGroup = generator.DeclareLocal(typeof(ManaGroup));

                

                BepinexPlugin.log.LogDebug("After:");
                foreach (var i in cm.InsertAndAdvance(
                    new CodeInstruction(OpCodes.Ldloc, l_manaGroup),
                    new CodeInstruction(OpCodes.Ldarg_3).WithLabels(labels),
                    new CodeInstruction(OpCodes.Ldarga_S, 2),
                    new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Nullable<ManaGroup>), nameof(Nullable<ManaGroup>.GetValueOrDefault))),
                    new CodeInstruction(OpCodes.Ldloc_S, 8),
                    new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(CardIdentity), nameof(CardIdentity.ManaMatchesCardIdentity))),
                    new CodeInstruction(OpCodes.Brfalse, tgtLabel)
                ).RemoveInstructions(9).InstructionEnumeration())
                {
                    //BepinexPlugin.log.LogDebug(i);
                    yield return i;
                }
            }
        }
    }
}
