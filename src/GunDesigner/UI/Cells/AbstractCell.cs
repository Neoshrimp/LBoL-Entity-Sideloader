using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UniverseLib.UI.Widgets.ScrollView;

namespace GunDesigner.UI.Cells
{
    public abstract class AbstractCell<T> : ICell where T : class
    {
        public float DefaultHeight => 30f;

        public GameObject UIRoot { get; set; }

        public bool Enabled => m_enabled;
        private bool m_enabled;


        public RectTransform Rect { get; set; }

        public void Disable()
        {
            m_enabled = false;
            UIRoot.SetActive(false);
        }

        public void Enable()
        {
            m_enabled = true;
            UIRoot.SetActive(true);
        }

        public abstract GameObject CreateContent(GameObject parent);
    }
}
