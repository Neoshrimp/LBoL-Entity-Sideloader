using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UI;
using UniverseLib.UI;
using UniverseLib.UI.Models;
using UniverseLib.Utility;

namespace GunDesigner.UI.Entries
{
    public abstract class TextEntry<T> : PropEntry<T> where T : class
    {

        public InputFieldRef InputField { get; set; }

        public virtual string DefaultValue { get => "..."; }

        public override GameObject MakeContent(GameObject parent)
        {
            UIRoot = base.MakeContent(parent);
            InputField = UIFactory.CreateInputField(rightHoriGroup, "InputField", DefaultValue);

            UIFactory.SetLayoutElement(InputField.UIRoot, minWidth: 150, flexibleWidth: 0, minHeight: 25, flexibleHeight: 0);
            InputField.OnValueChanged += ValChanged;


            return UIRoot;
        }

        protected void ValChanged(string value)
        {
            InputField.Text = value.ToString();
            UItoData();
        }

    }

    public abstract class IntEntry<T> : TextEntry<T> where T : class
    {

        public override string DefaultValue { get => "0"; }
        public override GameObject MakeContent(GameObject parent)
        {
            UIRoot = base.MakeContent(parent);

            InputField.GameObject.GetComponent<InputField>().contentType = UnityEngine.UI.InputField.ContentType.IntegerNumber;

            return UIRoot;
        }

        public int GetValue()
        {
            try
            {
                return int.Parse(InputField.Text);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void SetValue(int val) => InputField.Text = ParseUtility.ToStringForInput(val, typeof(int));
    }



    public abstract class FloatEntry<T> : TextEntry<T> where T : class
    {

        public override string DefaultValue { get => "0.0"; }

        public override GameObject MakeContent(GameObject parent)
        {
            UIRoot = base.MakeContent(parent);

            InputField.GameObject.GetComponent<InputField>().contentType = UnityEngine.UI.InputField.ContentType.DecimalNumber;

            return UIRoot;
        }

        public float GetValue()
        {
            try
            {
                return float.Parse(InputField.Text);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void SetValue(float val) => InputField.Text = ParseUtility.ToStringForInput(val, typeof(float));
    }


    public abstract class StringEntry<T> : TextEntry<T> where T : class
    {
        public override GameObject MakeContent(GameObject parent)
        {
            UIRoot = base.MakeContent(parent);

            InputField.GameObject.GetComponent<InputField>().contentType = UnityEngine.UI.InputField.ContentType.Standard;

            return UIRoot;
        }

        public string GetValue()
        {
            return InputField.Text;
        }

        public void SetValue(string val) => InputField.Text = val;
    }


}
