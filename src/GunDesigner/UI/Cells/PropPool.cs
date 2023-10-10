using LBoL.EntityLib.Cards.Character.Cirno;
using System;
using System.Collections.Generic;
using System.Text;
using UniverseLib.UI.Widgets.ScrollView;

namespace GunDesigner.UI.Cells
{
    public class PropPool<T> : ICellPoolDataSource<PropCell<T>> where T: class
    {

        public T target;

        public List<PropCell<T>> cells = new List<PropCell<T>>();

        public int ItemCount => cells.Count;

        public void OnCellBorrowed(PropCell<T> cell) { }

        public void SetCell(PropCell<T> cell, int index) 
        {

            if (index < 0 || index >= cells.Count)
                return;



            cell.target = this.target;
            cell.DataToUI(target);
        }


    }
}
