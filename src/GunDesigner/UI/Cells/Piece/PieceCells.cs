using GunDesigner.ConfigBuilders.Piece;
using System;
using System.Collections.Generic;
using System.Text;

namespace GunDesigner.UI.Cells.Piece
{
    public class addParentAngle : BoolCell<PieceReadableConfig>
    {
        public override void AssignAction(PieceReadableConfig target) => target.addParentAngle = GetValue();

        public override void DataToUI(PieceReadableConfig target) => SetValue(target.addParentAngle);
    }
}
