using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;
using UniverseLib.UI;
using UniverseLib.UI.Models;
using UniverseLib.Utility;

namespace GunDesigner.UI.Cells
{
    public class BoolCell<T> : PropCell<T> where T : class
    {

        public bool content;

        public Toggle Toggle;
        public Text ToggleText;

        public Text NameLabel;

        public LayoutElement NameLayout;
        public GameObject RightGroupContent;
        public LayoutElement RightGroupLayout;
        public GameObject SubContentHolder;

        public InputFieldRef HiddenNameLabel;

        public override GameObject CreateContent(GameObject parent)
        {

            UIRoot = base.CreateContent(parent);


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
            HiddenNameLabel.Component.lineType = InputField.LineType.MultiLineNewline;
            HiddenNameLabel.Component.textComponent.horizontalOverflow = HorizontalWrapMode.Wrap;
            HiddenNameLabel.Component.gameObject.GetComponent<Image>().color = Color.clear;
            HiddenNameLabel.Component.textComponent.color = Color.clear;
            UIFactory.SetLayoutElement(HiddenNameLabel.Component.gameObject, minHeight: 25, minWidth: 20, flexibleHeight: 300, flexibleWidth: 0);

            // Right vertical group

            RightGroupContent = UIFactory.CreateUIObject("RightGroup", horiRow);
            UIFactory.SetLayoutGroup<VerticalLayoutGroup>(RightGroupContent, false, false, true, true, 4, childAlignment: TextAnchor.UpperLeft);
            UIFactory.SetLayoutElement(RightGroupContent, minHeight: 25, minWidth: 200, flexibleWidth: 9999, flexibleHeight: 800);




            GameObject rightHoriGroup = UIFactory.CreateUIObject("RightHoriGroup", RightGroupContent);
            UIFactory.SetLayoutGroup<HorizontalLayoutGroup>(rightHoriGroup, false, false, true, true, 4, childAlignment: TextAnchor.UpperLeft);
            UIFactory.SetLayoutElement(rightHoriGroup, minHeight: 25, minWidth: 200, flexibleWidth: 9999, flexibleHeight: 800);

            GameObject toggleObj = UIFactory.CreateToggle(rightHoriGroup, "Toggle", out Toggle, out ToggleText);
            UIFactory.SetLayoutElement(toggleObj, minWidth: 70, minHeight: 25, flexibleWidth: 0, flexibleHeight: 0);
            ToggleText.color = SignatureHighlighter.KeywordBlue;
            Toggle.onValueChanged.AddListener(ToggleClicked);


            return UIRoot;
        }

        protected virtual void ToggleClicked(bool value)
        {
            ToggleText.text = value.ToString();
            AssignAction(target);
        }

        public bool GetValue() => Toggle.isOn;

        public void SetValue(bool val) => Toggle.isOn = val;
    }
}
