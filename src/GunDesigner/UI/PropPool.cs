using LBoL.EntityLib.Cards.Character.Cirno;
using System;
using System.Collections.Generic;
using System.Text;
using UniverseLib.UI.Widgets.ScrollView;

namespace GunDesigner.UI
{
    public class PropPool : ICellPoolDataSource<PropCell>
    {

        public List<PropCell> list = new List<PropCell>();

        public int ItemCount => list.Count;

        public void OnCellBorrowed(PropCell cell) { }

        public void SetCell(PropCell cell, int index) { }


    }
}
