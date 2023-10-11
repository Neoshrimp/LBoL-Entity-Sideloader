using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UniverseLib.UI.Widgets.ScrollView;
using UniverseLib.UI;
using LBoL.Presentation.UI;
using UnityEngine.UI;
using LBoL.EntityLib.Exhibits.Shining;

namespace GunDesigner.UI
{
    public class TestCell : ICell
    {
        public float DefaultHeight => 30f;

        public Text label;

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

        public GameObject CreateContent(GameObject parent)
        {
            UIRoot = UIFactory.CreateUIObject(this.GetType().Name, parent, new Vector2(100, 30));
            Rect = UIRoot.GetComponent<RectTransform>();
            UIFactory.SetLayoutGroup<VerticalLayoutGroup>(UIRoot, false, false, true, true, childAlignment: TextAnchor.UpperLeft);
            UIFactory.SetLayoutElement(UIRoot, minWidth: 100, flexibleWidth: 9999, minHeight: 30, flexibleHeight: 600);
            UIRoot.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            label = UIFactory.CreateLabel(UIRoot, "deez", "deeznuts");

            return UIRoot;
        }
    }

    public class TestCellHandler : GDPanelBase, ICellPoolDataSource<TestCell>
    {
        public GameObject panel;
        public GameObject panelContent;
        public ScrollPool<TestCell> ScrollPool;


        List<TestCell> cells = new List<TestCell>();

        public TestCellHandler(UIBase owner) : base(owner)
        {
        }

        public int ItemCount => cells.Count;

        public override UIMaster.Panels PanelType => UIMaster.Panels.Test;

        public override string Name => "Test Panel";

        public override int MinWidth => 300;

        public override int MinHeight => 600;

        public override Vector2 DefaultAnchorMin => new Vector2(0.5f, 1);

        public override Vector2 DefaultAnchorMax => new Vector2(0.5f, 1);

        public void OnCellBorrowed(TestCell cell) 
        {
            
        }

        public void SetCell(TestCell cell, int index)
        {

            if (index < 0 || index >= cells.Count)
            {
                return;
            }

            Log.log.LogDebug(index);
            var entry = cells[index];
            cell.label.text = "deeznuts";
            //entry.label.text = "nuts";
        }

        protected override void ConstructPanelContent()
        {

            cells.Add(new TestCell());
            cells.Add(new TestCell());
            cells.Add(new TestCell());

            //UIFactory.CreateScrollView

            ScrollPool = UIFactory.CreateScrollPool<TestCell>(panelContent, "MyScrollPool", out GameObject scrollRoot, out GameObject scrollContent, Color.blue);
            ScrollPool.Initialize(this);
        }

        // Implement interface members...
    }
}
