using LBoL.Core.Units;
using System;
using System.Collections.Generic;
using System.Text;

namespace LBoLEntitySideloader.BattleModifiers
{
    public delegate bool ModPrecond(Unit unit);
    public delegate Unit ModUnit(Unit unit);
}
