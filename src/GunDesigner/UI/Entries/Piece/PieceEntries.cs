using GunDesigner.ConfigBuilders.Piece;
using GunDesigner.UI.Entries;
using LBoL.ConfigData;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using UniverseLib.Utility;

namespace GunDesigner.UI.Entries.Piece
{

    public class Id : IntEntry<PieceReadableConfig>
    {
        public override void UItoData() => target.Id = GetValue();
        public override void DataToUI() => SetValue(target.Id);
    }

    public class projectile : StringEntry<PieceReadableConfig>
    {
        public override void UItoData() => target.projectile = GetValue();
        public override void DataToUI() => SetValue(target.projectile);
    }

    public class parentPiece : IntEntry<PieceReadableConfig>
    {
        public override void UItoData() => target.parentPiece = GetValue();
        public override void DataToUI() => SetValue(target.parentPiece);
    }

    public class lastWave : BoolEntry<PieceReadableConfig>
    {
        public override void UItoData() => target.lastWave = GetValue();

        public override void DataToUI() => SetValue(target.lastWave);
    }

    public class followPiece : IntEntry<PieceReadableConfig>
    {
        public override void UItoData() => target.followPiece = GetValue();
        public override void DataToUI() => SetValue(target.followPiece);
    }

    public class shootEnd : IntEntry<PieceReadableConfig>
    {
        public override void UItoData() => target.shootEnd = GetValue();
        public override void DataToUI() => SetValue(target.shootEnd);
    }

    public class hitAmount : IntEntry<PieceReadableConfig>
    {
        public override void UItoData() => target.hitAmount = GetValue();
        public override void DataToUI() => SetValue(target.hitAmount);
    }

    public class laserHitInterval : IntEntry<PieceReadableConfig>
    {
        public override void UItoData() => target.laserHitInterval = GetValue();
        public override void DataToUI() => SetValue(target.laserHitInterval);
    }

    public class zeroHitNotDie : BoolEntry<PieceReadableConfig>
    {
        public override void UItoData() => target.zeroHitNotDie = GetValue();
        public override void DataToUI() => SetValue(target.zeroHitNotDie);
    }



    public class scale : FloatEntry<PieceReadableConfig>
    {
        public override void UItoData() => target.scale = GetValue();

        public override void DataToUI() => SetValue(target.scale);
    }


    public class addParentAngle : BoolEntry<PieceReadableConfig>
    {
        public override void UItoData() => target.addParentAngle = GetValue();

        public override void DataToUI() => SetValue(target.addParentAngle);
    }

}
