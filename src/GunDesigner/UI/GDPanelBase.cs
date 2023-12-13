using LBoL.Presentation.UI;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UniverseLib.UI;
using UniverseLib.UI.Panels;

namespace GunDesigner.UI
{
    public abstract class GDPanelBase : PanelBase
    {
        protected GDPanelBase(UIBase owner) : base(owner) {}


        public abstract UIMaster.Panels PanelType { get; }


        public override void ConstructUI()
        {
            base.ConstructUI();
        }

    }
}
