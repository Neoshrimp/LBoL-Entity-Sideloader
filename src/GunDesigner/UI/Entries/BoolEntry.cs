using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;
using UniverseLib.UI;
using UniverseLib.UI.Models;
using UniverseLib.Utility;

namespace GunDesigner.UI.Entries
{
    public abstract class BoolEntry<T> : PropEntry<T> where T : class
    {

        public bool content;

        public Toggle Toggle;
        public Text ToggleText;



        public override GameObject MakeContent(GameObject parent)
        {

            UIRoot = base.MakeContent(parent);

            // toggle
            GameObject toggleObj = UIFactory.CreateToggle(rightHoriGroup, "Toggle", out Toggle, out ToggleText);
            UIFactory.SetLayoutElement(toggleObj, minWidth: 70, minHeight: 25, flexibleWidth: 0, flexibleHeight: 0);
            ToggleText.color = SignatureHighlighter.KeywordBlue;
            Toggle.onValueChanged.AddListener(ToggleClicked);


            return UIRoot;
        }

        protected void ToggleClicked(bool value)
        {
            ToggleText.text = value.ToString();
            AssignAction(target);
        }

        public bool GetValue() => Toggle.isOn;

        public void SetValue(bool val) => Toggle.isOn = val;
    }
}
