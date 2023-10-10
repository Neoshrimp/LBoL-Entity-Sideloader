using GunDesigner.ConfigBuilders.Piece;
using System;
using System.Collections.Generic;
using System.Text;

namespace GunDesigner.UI.Cells.Piece
{
    public class PiecePropPool : PropPool<PieceReadableConfig>
    {
        public PiecePropPool()
        {
            cells.Add(new addParentAngle());
        }
    }
}
