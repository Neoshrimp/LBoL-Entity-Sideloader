using HarmonyLib;
using LBoL.Core.Units;
using System.Collections.Generic;

namespace LBoLEntitySideloader.BattleModifiers
{
    public class UnitModTracker
    {
        static UnitModTracker _instance;
        static public UnitModTracker Instance { get { _instance ??= new UnitModTracker(); return _instance; } }

        private Dictionary<string, HashSet<UnitModifier>> modDic = new Dictionary<string, HashSet<UnitModifier>>();

        public void AddModifier(UnitModifier modifier)
        {
            modDic.TryAdd(modifier.unitId, new HashSet<UnitModifier>());
            modDic[modifier.unitId].Add(modifier);
        }

        public void Perform(Unit unit)
        {
            if (modDic.TryGetValue(unit.Id, out var modifiers))
            {
                modifiers.Do(m => m.ApplyMods(unit));
            }
        }
    }
}
