using HarmonyLib;
using LBoL.Core;
using LBoL.Core.Units;
using LBoL.EntityLib.EnemyUnits.Character;
using LBoL.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using UnityEngine;

namespace LBoLEntitySideloader.Entities.Patches
{
    [HarmonyPatch(typeof(Siji), nameof(Siji.OnBattleStarted), MethodType.Enumerator)]
    class EikiSummonPatch
    {
        private static void CheckSummon(ref Type summonType, ref string spellcardId)
        {

            var currentPcId = GameMaster.Instance.CurrentGameRun.Player.Id;

            var currentPcTemplate = EntityManager.Instance.AllUsers.Select(tu => tu.userInfo)
                .SelectMany(ui => ui.definitionInstances.Values)
                .Where(d => d is PlayerUnitTemplate)
                .Cast<PlayerUnitTemplate>()
                .FirstOrDefault(put => currentPcId == put.UniqueId);

            if (currentPcTemplate != null)
            {
                try
                {
                    var si = currentPcTemplate.AssociateEikiSummon();
                    if (si != null)
                    {
                        if (!si.summonType.IsSubclassOf(typeof(EnemyUnit)))
                            throw new InvalidOperationException($"{si.summonType.Name} is not subclass of {nameof(EnemyUnit)}");
                        summonType = si.summonType;
                        spellcardId = si.spellcardId;
                    }

                }
                catch (Exception e)
                {
                    BepinexPlugin.log.LogError($"Error invoking {currentPcTemplate.GetType().Name}.{nameof(PlayerUnitTemplate.AssociateEikiSummon)}: {e}");
                }
            }

        }

        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, MethodBase originalMethod)
        {
            var enumType = originalMethod.DeclaringType;
            var unitTypeField = AccessTools.GetDeclaredFields(enumType).First(fi => fi.Name.Contains("type"));
            var spellcardIdField = AccessTools.GetDeclaredFields(enumType).First(fi => fi.Name.Contains("spellcard"));



            return new CodeMatcher(instructions)
            .MatchEndForward(new CodeInstruction(OpCodes.Ldstr, "Chat.Siji1"))
            .InsertAndAdvance(new CodeInstruction(OpCodes.Ldarg_0))
            .InsertAndAdvance(new CodeInstruction(OpCodes.Ldflda, unitTypeField))
            .InsertAndAdvance(new CodeInstruction(OpCodes.Ldarg_0))
            .InsertAndAdvance(new CodeInstruction(OpCodes.Ldflda, spellcardIdField))
            .InsertAndAdvance(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(EikiSummonPatch), nameof(EikiSummonPatch.CheckSummon))))

            .InstructionEnumeration();
        }

    }
}
