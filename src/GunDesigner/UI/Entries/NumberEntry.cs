using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UniverseLib.UI;
using UniverseLib.UI.Models;

namespace GunDesigner.UI.Entries
{
    public abstract class NumberEntry<T> : PropEntry<T> where T : class
    {

        InputFieldRef InputField { get; set; }

        public override GameObject MakeContent(GameObject parent)
        {
            UIRoot = base.MakeContent(parent);
            InputField = UIFactory.CreateInputField(rightHoriGroup, "InputField", "...");

            UIFactory.SetLayoutElement(InputField.UIRoot, minWidth: 150, flexibleWidth: 0, minHeight: 25, flexibleHeight: 0);
            InputField.OnValueChanged += ValChanged;

            InputField.GameObject.GetComponent<InputField>().contentType = UnityEngine.UI.InputField.ContentType.IntegerNumber;

            return UIRoot;
        }

        protected void ValChanged(string value)
        {
            InputField.Text = value.ToString();
            UItoData();
        }

        public string GetValue()
        {
            return InputField.Text;
        }

        public void SetValue(string val) => InputField.Text = val;
    }
}
