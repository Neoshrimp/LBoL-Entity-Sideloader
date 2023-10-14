using GunDesigner.ConfigBuilders.Piece;
using GunDesigner.UI.Entries;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using UniverseLib.Utility;

namespace GunDesigner.UI.Entries.Piece
{

    public class Id : NumberEntry<PieceReadableConfig>
    {
        public override void UItoData()
        {
            try
            {
                target.Id = int.Parse(GetValue());
            }
            catch (Exception)
            {
            }
        }

        
        public override void DataToUI() => SetValue(ParseUtility.ToStringForInput<int>(target.Id));

    }

    public class addParentAngle : BoolEntry<PieceReadableConfig>
    {
        public override void UItoData() => target.addParentAngle = GetValue();

        public override void DataToUI() => SetValue(target.addParentAngle);
    }
}
