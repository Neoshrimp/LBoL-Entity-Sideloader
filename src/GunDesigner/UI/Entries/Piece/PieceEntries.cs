using GunDesigner.ConfigBuilders.Piece;
using GunDesigner.UI.Entries;
using System;
using System.Collections.Generic;
using System.Text;

namespace GunDesigner.UI.Entries.Piece
{
    public class addParentAngle : BoolEntry<PieceReadableConfig>
    {
        public override void AssignAction(PieceReadableConfig target) => target.addParentAngle = GetValue();

        public override void DataToUI(PieceReadableConfig target) => SetValue(target.addParentAngle);
    }
}
