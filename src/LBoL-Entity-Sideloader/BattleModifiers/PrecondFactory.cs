using LBoL.Core.Units;
using LBoL.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LBoLEntitySideloader.BattleModifiers
{
    public static class PrecondFactory
    {


        public static ModPrecond HasJadeboxes(HashSet<string> jadeBoxes)
        {
            return (unit) => unit.GameRun.JadeBoxes.Select(jb => jb.GetType().Name).Count(name => jadeBoxes.Contains(name)) >= jadeBoxes.Count;
        }

        static public ModPrecond IsEnemyGroup(string groupId)
        {
            return (unit) => groupId != null && GameMaster.Instance?.CurrentGameRun?.Battle?.EnemyGroup?.Id == groupId;
        }

        static public ModPrecond IsStage(Type stageType)
        {
            return (unit) => stageType != null && GameMaster.Instance?.CurrentGameRun?.CurrentStage?.GetType() == stageType;
        }
    }
}