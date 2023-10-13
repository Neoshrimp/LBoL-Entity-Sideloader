using GunDesigner.ConfigBuilders.Piece;
using GunDesigner.UI.Cells;
using GunDesigner.UI.Cells.Piece;
using LBoL.Presentation.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityExplorer.Core;
using UnityExplorer.Core.CacheObject;
using UnityExplorer.Core.CacheObject.Views;
using UnityExplorer.Core.Inspectors.MouseInspectors;
using UniverseLib.UI;
using UniverseLib.UI.Panels;
using UniverseLib.UI.Widgets.ScrollView;

namespace GunDesigner.UI
{
    public class PiecePanel : GDPanelBase, ICellPoolDataSource<PropCell<PieceReadableConfig>>
    {


        public override string Name => "Piece Builder";

        public override int MinWidth => 450;

        public override int MinHeight => 800;

        public override Vector2 DefaultAnchorMin => new Vector2(0.5f, 1f);

        public override Vector2 DefaultAnchorMax => new Vector2(0.5f, 1f);

        public override UIMaster.Panels PanelType => UIMaster.Panels.Piece;


        public Dictionary<int, PieceReadableConfig> configs = new Dictionary<int, PieceReadableConfig>() { {0, new PieceReadableConfig() } };

        //public PropPool<PieceReadableConfig> piecePropPool = new PropPool<PieceReadableConfig>();


        /*        public class ConfigPoolPair
                {
                    public PieceReadableConfig pieceReadableConfig;
                    public PiecePropPool piecePropPool;
                }*/

        List<PropCell<PieceReadableConfig>> cellData = new List<PropCell<PieceReadableConfig>>();

        public int ItemCount => cellData.Count;


        public void OnCellBorrowed(PropCell<PieceReadableConfig> cell) {}

        public void SetCell(PropCell<PieceReadableConfig> cell, int index)
        {
            //cell.target = configs.First().Value;
        }



        public PiecePanel(UIBase owner) : base(owner)
        {
            UIMaster.panelManager.OnClickedOutsidePanels += Unfocused;



            
            cellData.Add(new PropCell<PieceReadableConfig>());

/*            piecePropPool.target = configs.First().Value;
            piecePropPool.cells.Add(new PropCell<PieceReadableConfig>());*/


            //piecePropPool.cells.Add(new addParentAngle());

        }


        public void Unfocused()
        {
            if (this.Enabled)
            {
                Log.log.LogInfo("Outside d3eez");
            
            }
        }

        public override void SetDefaultSizeAndPosition()
        {
            base.SetDefaultSizeAndPosition();
            this.Rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, MinWidth);
            this.Rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, MinHeight);
        }

        protected override void ConstructPanelContent()
        {
            GameObject firstRow = UIFactory.CreateHorizontalGroup(
                parent: ContentRoot,
                name: "FirstRow",
                forceExpandWidth: false,
                forceExpandHeight: false,
                childControlWidth: true,
                childControlHeight: true,
                spacing: 5,
                padding: new Color(2, 2, 2, 2),
                bgColor: new Color(1, 1, 1, 0));

            UIFactory.SetLayoutElement(firstRow, minHeight: 25, flexibleWidth: 999);

            Text title = UIFactory.CreateLabel(firstRow, "Deez", "Nuts", TextAnchor.MiddleLeft, color: Color.grey);

            UIFactory.SetLayoutElement(title.gameObject, minHeight: 25, minWidth: 100, flexibleWidth: 999);

            ScrollPool<PropCell<PieceReadableConfig>> scrollPool = UIFactory.CreateScrollPool<PropCell<PieceReadableConfig>>(
                this.ContentRoot,
                "PieceEntries",
                out GameObject scrollObj,
                out GameObject scrollContent);

            scrollPool.Initialize(this);

        }


    }
}
