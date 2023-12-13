using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UniverseLib.UI.Widgets.ScrollView;
using UniverseLib.UI;
using LBoL.Presentation.UI;
using UnityEngine.UI;
using LBoL.EntityLib.Exhibits.Shining;
using UniverseLib.UI.Models;
using UnityExplorer.Core.CacheObject.Views;
using UnityExplorer.Core.CacheObject;
using UnityExplorer.Core.Config;
using UnityExplorer.Core.UI.Panels;
using UnityExplorer.Core.UI;
using System.Linq;

namespace GunDesigner.UI
{
    public class TestCell : ICell
    {
        public float DefaultHeight => 30f;

        public Text label;

        public GameObject UIRoot { get; set; }

        public bool Enabled => m_enabled;
        private bool m_enabled;


        public bool SubContentActive => SubContentHolder.activeSelf;

        public LayoutElement NameLayout;
        public GameObject RightGroupContent;
        public LayoutElement RightGroupLayout;
        public GameObject SubContentHolder;

        public Text NameLabel;
        public InputFieldRef HiddenNameLabel; // for selecting the name label
        public Text TypeLabel;
        public Text ValueLabel;
        public Toggle Toggle;
        public Text ToggleText;
        public InputFieldRef InputField;

        public ButtonRef InspectButton;
        public ButtonRef SubContentButton;
        public ButtonRef ApplyButton;

        public ButtonRef CopyButton;
        public ButtonRef PasteButton;

        public readonly Color subInactiveColor = new Color(0.23f, 0.23f, 0.23f);
        public readonly Color subActiveColor = new Color(0.23f, 0.33f, 0.23f);


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

            GameObject horiRow = UIFactory.CreateUIObject("HoriGroup", UIRoot);
            UIFactory.SetLayoutElement(horiRow, minHeight: 29, flexibleHeight: 150, flexibleWidth: 9999);
            UIFactory.SetLayoutGroup<HorizontalLayoutGroup>(horiRow, false, false, true, true, 5, 2, childAlignment: TextAnchor.UpperLeft);
            horiRow.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            // Left name label

            NameLabel = UIFactory.CreateLabel(horiRow, "NameLabel", "<notset>", TextAnchor.MiddleLeft);
            NameLabel.horizontalOverflow = HorizontalWrapMode.Wrap;
            NameLayout = UIFactory.SetLayoutElement(NameLabel.gameObject, minHeight: 25, minWidth: 20, flexibleHeight: 300, flexibleWidth: 0);
            UIFactory.SetLayoutGroup<VerticalLayoutGroup>(NameLabel.gameObject, true, true, true, true);

            HiddenNameLabel = UIFactory.CreateInputField(NameLabel.gameObject, "HiddenNameLabel", "");
            RectTransform hiddenRect = HiddenNameLabel.Component.GetComponent<RectTransform>();
            hiddenRect.anchorMin = Vector2.zero;
            hiddenRect.anchorMax = Vector2.one;
            HiddenNameLabel.Component.readOnly = true;
            HiddenNameLabel.Component.lineType = UnityEngine.UI.InputField.LineType.MultiLineNewline;
            HiddenNameLabel.Component.textComponent.horizontalOverflow = HorizontalWrapMode.Wrap;
            HiddenNameLabel.Component.gameObject.GetComponent<Image>().color = Color.clear;
            HiddenNameLabel.Component.textComponent.color = Color.clear;
            UIFactory.SetLayoutElement(HiddenNameLabel.Component.gameObject, minHeight: 25, minWidth: 20, flexibleHeight: 300, flexibleWidth: 0);

            // Right vertical group

            RightGroupContent = UIFactory.CreateUIObject("RightGroup", horiRow);
            UIFactory.SetLayoutGroup<VerticalLayoutGroup>(RightGroupContent, false, false, true, true, 4, childAlignment: TextAnchor.UpperLeft);
            UIFactory.SetLayoutElement(RightGroupContent, minHeight: 25, minWidth: 200, flexibleWidth: 9999, flexibleHeight: 800);
            RightGroupLayout = RightGroupContent.GetComponent<LayoutElement>();


            // Right horizontal group

            GameObject rightHoriGroup = UIFactory.CreateUIObject("RightHoriGroup", RightGroupContent);
            UIFactory.SetLayoutGroup<HorizontalLayoutGroup>(rightHoriGroup, false, false, true, true, 4, childAlignment: TextAnchor.UpperLeft);
            UIFactory.SetLayoutElement(rightHoriGroup, minHeight: 25, minWidth: 200, flexibleWidth: 9999, flexibleHeight: 800);

            SubContentButton = UIFactory.CreateButton(rightHoriGroup, "SubContentButton", "▲", subInactiveColor);
            UIFactory.SetLayoutElement(SubContentButton.Component.gameObject, minWidth: 25, minHeight: 25, flexibleWidth: 0, flexibleHeight: 0);

            // Type label

            TypeLabel = UIFactory.CreateLabel(rightHoriGroup, "ReturnLabel", "<notset>", TextAnchor.MiddleLeft);
            TypeLabel.horizontalOverflow = HorizontalWrapMode.Wrap;
            UIFactory.SetLayoutElement(TypeLabel.gameObject, minHeight: 25, flexibleHeight: 150, minWidth: 45, flexibleWidth: 0);

            // Bool and number value interaction

            GameObject toggleObj = UIFactory.CreateToggle(rightHoriGroup, "Toggle", out Toggle, out ToggleText);
            UIFactory.SetLayoutElement(toggleObj, minWidth: 70, minHeight: 25, flexibleWidth: 0, flexibleHeight: 0);

            InputField = UIFactory.CreateInputField(rightHoriGroup, "InputField", "...");
            UIFactory.SetLayoutElement(InputField.UIRoot, minWidth: 150, flexibleWidth: 0, minHeight: 25, flexibleHeight: 0);

            // Apply

            ApplyButton = UIFactory.CreateButton(rightHoriGroup, "ApplyButton", "Apply", new Color(0.15f, 0.19f, 0.15f));
            UIFactory.SetLayoutElement(ApplyButton.Component.gameObject, minWidth: 70, minHeight: 25, flexibleWidth: 0, flexibleHeight: 0);

            // Inspect 

            InspectButton = UIFactory.CreateButton(rightHoriGroup, "InspectButton", "Inspect", new Color(0.15f, 0.15f, 0.15f));
            UIFactory.SetLayoutElement(InspectButton.Component.gameObject, minWidth: 70, flexibleWidth: 0, minHeight: 25);

            // Main value label

            ValueLabel = UIFactory.CreateLabel(rightHoriGroup, "ValueLabel", "Value goes here", TextAnchor.MiddleLeft);
            ValueLabel.horizontalOverflow = HorizontalWrapMode.Wrap;
            UIFactory.SetLayoutElement(ValueLabel.gameObject, minHeight: 25, flexibleHeight: 150, flexibleWidth: 9999);

            // Copy and Paste buttons

            GameObject buttonHolder = UIFactory.CreateHorizontalGroup(rightHoriGroup, "CopyPasteButtons", false, false, true, true, 4,
                bgColor: new Color(1, 1, 1, 0), childAlignment: TextAnchor.MiddleLeft);
            UIFactory.SetLayoutElement(buttonHolder, minWidth: 60, flexibleWidth: 0);

            CopyButton = UIFactory.CreateButton(buttonHolder, "CopyButton", "Copy", new Color(0.13f, 0.13f, 0.13f, 1f));
            UIFactory.SetLayoutElement(CopyButton.Component.gameObject, minHeight: 25, minWidth: 28, flexibleWidth: 0);
            CopyButton.ButtonText.color = Color.yellow;
            CopyButton.ButtonText.fontSize = 10;

            PasteButton = UIFactory.CreateButton(buttonHolder, "PasteButton", "Paste", new Color(0.13f, 0.13f, 0.13f, 1f));
            UIFactory.SetLayoutElement(PasteButton.Component.gameObject, minHeight: 25, minWidth: 28, flexibleWidth: 0);
            PasteButton.ButtonText.color = Color.green;
            PasteButton.ButtonText.fontSize = 10;

            // Subcontent

            SubContentHolder = UIFactory.CreateUIObject("SubContent", UIRoot);
            UIFactory.SetLayoutElement(SubContentHolder.gameObject, minHeight: 30, flexibleHeight: 600, minWidth: 100, flexibleWidth: 9999);
            UIFactory.SetLayoutGroup<VerticalLayoutGroup>(SubContentHolder, true, true, true, true, 2, childAlignment: TextAnchor.UpperLeft);
            //SubContentHolder.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.MinSize;
            SubContentHolder.SetActive(false);

            // Bottom separator
            GameObject separator = UIFactory.CreateUIObject("BottomSeperator", UIRoot);
            UIFactory.SetLayoutElement(separator, minHeight: 1, flexibleHeight: 0, flexibleWidth: 9999);
            separator.AddComponent<Image>().color = Color.black;

            return UIRoot;
        }
    }

    public class TestCellHandler : GDPanelBase, ICellPoolDataSource<TestCell>
    {
        public GameObject panel;
        public GameObject panelContent;
        public ScrollPool<TestCell> ScrollPool;


        List<TestCell> cellData = new List<TestCell>();

        public TestCellHandler(UIBase owner) : base(owner)
        {

            cellData.Add(new TestCell());
            cellData.Add(new TestCell());

        }

        public int ItemCount => cellData.Count;

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

            if (index < 0 || index >= cellData.Count)
            {
                return;
            }

            Log.log.LogDebug(index);
            
            cell.NameLabel.text = "deeznuts";
            //entry.label.text = "nuts";
        }

        protected override void ConstructPanelContent()
        {


            //UIFactory.CreateScrollView

            ScrollPool = UIFactory.CreateScrollPool<TestCell>(ContentRoot, "MyScrollPool", out GameObject scrollRoot, out GameObject scrollContent, Color.blue);
            ScrollPool.Initialize(this);

            // Clear clipboard button
            UniverseLib.UI.Models.ButtonRef clearButton = UIFactory.CreateButton(uiRoot, "ClearPasteButton", "Clear Clipboard");
            UIFactory.SetLayoutElement(clearButton.Component.gameObject, minWidth: 120, minHeight: 25, flexibleWidth: 0);
            clearButton.OnClick += () => Log.log.LogInfo("deeeez");

            // Current Paste info row
            GameObject currentPasteHolder = UIFactory.CreateHorizontalGroup(ContentRoot, "SecondRow", false, false, true, true, 0,
                new Vector4(2, 2, 2, 2), childAlignment: TextAnchor.UpperCenter);
        }


    }



    public class TestCellHandlerDeez : GDPanelBase, ICellPoolDataSource<TestCell>
    {
        public override string Name => "Options";
        public override UIMaster.Panels PanelType => UIMaster.Panels.Test;

        public override int MinWidth => 600;
        public override int MinHeight => 200;
        public override Vector2 DefaultAnchorMin => new Vector2(0.5f, 0.1f);
        public override Vector2 DefaultAnchorMax => new Vector2(0.5f, 0.85f);



        // Entry holders
        //private readonly List<CacheConfigEntry> configEntries = new List<CacheConfigEntry>();

        private readonly List<TestCell> configEntries = new List<TestCell>();




        // ICellPoolDataSource
        public int ItemCount => configEntries.Count;


        public TestCellHandlerDeez(UIBase owner) : base(owner)
        {
            foreach (KeyValuePair<string, IConfigElement> entry in ConfigManager.ConfigElements.Where((e, i) => i<5))
            {
                /*                CacheConfigEntry cache = new CacheConfigEntry(entry.Value)
                                {
                                    Owner = null
                                };
                                configEntries.Add(cache);*/

                configEntries.Add(new TestCell());
            }

/*            foreach (CacheConfigEntry config in configEntries)
                config.UpdateValueFromSource();*/
        }

        public void OnCellBorrowed(TestCell cell)
        {
        }

        public void SetCell(TestCell cell, int index)
        {
            if (index < 0 || index >= configEntries.Count)
            {
/*                if (cell.Occupant != null)
                    cell.Occupant.UnlinkFromView();

                cell.Disable();*/
                return;
            }

            //CacheObjectBase entry = (CacheObjectBase)configEntries[index];

/*            if (entry.CellView != null && entry.CellView != cell)
                entry.UnlinkFromView();

            if (cell.Occupant != null && cell.Occupant != entry)
                cell.Occupant.UnlinkFromView();

            if (entry.CellView != cell)
                entry.SetView(cell);*/

            //entry.SetDataToCell(cell);

        }

        // UI Construction

        public override void SetDefaultSizeAndPosition()
        {
            base.SetDefaultSizeAndPosition();

            Rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 600f);
        }

        protected override void ConstructPanelContent()
        {
            // Save button

            UniverseLib.UI.Models.ButtonRef saveBtn = UIFactory.CreateButton(this.ContentRoot, "Save", "Save Options", new Color(0.2f, 0.3f, 0.2f));
            UIFactory.SetLayoutElement(saveBtn.Component.gameObject, flexibleWidth: 9999, minHeight: 30, flexibleHeight: 0);
            
            
            //saveBtn.OnClick += ConfigManager.Handler.SaveConfig;

            // Config entries

/*            ScrollPool<ConfigEntryCell> scrollPool = UIFactory.CreateScrollPool<ConfigEntryCell>(
                this.ContentRoot,
                "ConfigEntries",
                out GameObject scrollObj,
                out GameObject scrollContent);*/


            ScrollPool<TestCell> scrollPool = UIFactory.CreateScrollPool<TestCell>(
                this.ContentRoot,
                "ConfigEntries",
                out GameObject scrollObj,
                out GameObject scrollContent);

            scrollPool.Initialize(this);
        }
    }
}
