using System;
using System.Collections.Generic;
using System.Text;
using UniverseLib.UI;
using UniverseLib.UI.Panels;

namespace GunDesigner.UI
{
    internal class GDUIBase : UIBase
    {
        public GDUIBase(string id, Action updateMethod) : base(id, updateMethod) {}

        protected override PanelManager CreatePanelManager()
        {
            return new GDPanelManager(this);       
        }
    }
}
