using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;
using UniverseLib.UI;
using UniverseLib.UI.Widgets.ScrollView;

namespace GunDesigner.UI.Cells
{
    public class PropCell<T> : AbstractCell<T> where T : class
    {

        public T target; 

    

        public override GameObject CreateContent(GameObject parent)
        {    
            // Main layout
            UIRoot = UIFactory.CreateUIObject(this.GetType().Name, parent, new Vector2(100, 30));
            Rect = UIRoot.GetComponent<RectTransform>();
            UIFactory.SetLayoutGroup<VerticalLayoutGroup>(UIRoot, false, false, true, true, childAlignment: TextAnchor.UpperLeft);
            UIFactory.SetLayoutElement(UIRoot, minWidth: 100, flexibleWidth: 9999, minHeight: 30, flexibleHeight: 600);
            UIRoot.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            return UIRoot;
        }

        public virtual void AssignAction(T target) { }

        public virtual void DataToUI(T target) { }

    }
}
