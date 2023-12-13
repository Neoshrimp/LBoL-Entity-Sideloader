using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UniverseLib.UI;

namespace GunDesigner.UI.Entries
{
    public abstract class EnumEntry<T> : PropEntry<T> where T : class
    {

        public Enum @enum;


        public override GameObject MakeContent(GameObject parent)
        {
            UIRoot = base.MakeContent(parent);

            SubContentHolder = UIFactory.CreateUIObject("SubContent", UIRoot);
            UIFactory.SetLayoutElement(SubContentHolder.gameObject, minHeight: 30, flexibleHeight: 600, minWidth: 100, flexibleWidth: 9999);
            UIFactory.SetLayoutGroup<VerticalLayoutGroup>(SubContentHolder, true, true, true, true, 2, childAlignment: TextAnchor.UpperLeft);

            return UIRoot;
        }



    }
}
