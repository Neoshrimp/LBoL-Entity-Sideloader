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
using LBoL.EntityLib.Cards.Character.Reimu;

namespace LBoLEntitySideloader.GameFixes
{
    // dev fixed
    //[HarmonyPatch(typeof(ReimuSilence), "Actions", MethodType.Enumerator)]
    class ReimuSilencePatch
    {

        private static int CheckUpgrade(ReimuSilence reimuSilence)
        {
            return reimuSilence.IsUpgraded ? reimuSilence.Config.UpgradedDamage.Value : reimuSilence.Config.Damage.Value;
        }

        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var matcher = new CodeMatcher(instructions);

            matcher.MatchEndForward(new CodeInstruction(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(CardConfig), nameof(CardConfig.Damage))));

            if (matcher.IsValid)
                matcher.MatchEndForward(new CodeInstruction(OpCodes.Call, AccessTools.PropertyGetter(typeof(Nullable<int>), nameof(Nullable<int>.Value))))
                .Advance(1)
                .InsertAndAdvance(new CodeInstruction(OpCodes.Pop))
                .InsertAndAdvance(new CodeInstruction(OpCodes.Ldloc_1))
                .InsertAndAdvance(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ReimuSilencePatch), nameof(ReimuSilencePatch.CheckUpgrade))))

                ;


            return matcher.InstructionEnumeration();
        }

        
    }
}
