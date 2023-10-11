using GunDesigner.Reflection;
using LBoL.Presentation.UI;
using LBoL.Presentation.UI.Panels;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UniverseLib.UI;
using UniverseLib.UI.Panels;

namespace GunDesigner.UI
{
    public static class UIMaster
    {

        public static Dictionary<Panels, GDPanelBase> AllPanels = new Dictionary<Panels, GDPanelBase>();

        public static GDPanelManager panelManager;

        public static UIBase uIBase;

        public static GameObject uIRoot => uIBase?.RootObject;
        public static RectTransform uIRootRect;
        public static Canvas uICanvas;

        public static void Init()
        {

            uIBase = UniversalUI.RegisterUI<GDUIBase>(PInfo.GUID, Update);

            panelManager = (GDPanelManager)uIBase.Panels;
            uIRootRect = uIRoot.GetComponent<RectTransform>();
            uICanvas = uIRoot.GetComponent<Canvas>();

            //AllPanels.Add(Panels.Piece, new PiecePanel(uIBase));

            AllPanels.Add(Panels.Test, new TestCellHandler(uIBase));

        }

        public static PanelBase GetPanel(Panels panel) => AllPanels[panel];

        public static T GetPanel<T>(Panels panel) where T : GDPanelBase => (T)AllPanels[panel];

        // does NOT get hot reloaded
        public static void Update()
        { }


        public static void DestroyAll()
        {
            foreach (var p in AllPanels.Values)
            {
                p.Destroy();
            }
            UniverseLibReflection.registeredUIsRef().Remove(PInfo.GUID);
            UniverseLibReflection.uiBases().Remove(uIBase);
            GameObject.Destroy(uIBase.RootObject);

        }

        public enum Panels
        {
            Piece,
            Test
        }
    }
}
