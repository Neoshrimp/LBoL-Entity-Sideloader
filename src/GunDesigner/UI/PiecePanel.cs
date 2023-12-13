using GunDesigner.ConfigBuilders.Piece;
using GunDesigner.UI.Entries;
using GunDesigner.UI.Entries.Piece;
using LBoL.EntityLib.Exhibits.Shining;
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
    public class PiecePanel : GDPanelBase
    {
        public override string Name => "Piece Builder";
        public override UIMaster.Panels PanelType => UIMaster.Panels.Piece;

        public override int MinWidth => 450;

        public override int MinHeight => 800;

        public override Vector2 DefaultAnchorMin => new Vector2(0.5f, 1f);

        public override Vector2 DefaultAnchorMax => new Vector2(0.5f, 1f);



        public Dictionary<int, PieceReadableConfig> configs = new Dictionary<int, PieceReadableConfig>() { {0, new PieceReadableConfig() } };

        List<PropEntry<PieceReadableConfig>> cellData = new List<PropEntry<PieceReadableConfig>>();

        public PieceReadableConfig tempTarget;

        public List<PropEntry<PieceReadableConfig>> uiEntries = new List<PropEntry<PieceReadableConfig>>()
        {
            new Id(),
            // type
            new projectile(),
            //
            new addParentAngle(),
            new lastWave(),
            new followPiece(),
            new shootEnd(),
            new hitAmount(),
            new laserHitInterval(),
            new zeroHitNotDie(),
            new scale()

        };

        public PiecePanel(UIBase owner) : base(owner)
        {
            UIMaster.panelManager.OnClickedOutsidePanels += Unfocused;





            /*            piecePropPool.target = configs.First().Value;
                        piecePropPool.cells.Add(new PropCell<PieceReadableConfig>());*/


            //piecePropPool.cells.Add(new addParentAngle());

        }


        public void Unfocused()
        {
            if (this.Enabled)
            {

                foreach (var e in uiEntries)
                {
                    e.UItoData();
                }
                Log.log.LogInfo(tempTarget.ConvertSelf());
            
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

            var scrollView = UIFactory.CreateScrollView(ContentRoot, "pieceScroll", out var scrollContent, out var autoSliderScrollbar);

            UIFactory.SetLayoutElement(scrollView, flexibleWidth: 9999, flexibleHeight: 9999);
            UIFactory.SetLayoutGroup<VerticalLayoutGroup>(scrollContent, padTop: 5, padLeft: 5, padBottom: 5, padRight: 5);

            this.tempTarget = configs.First().Value;


            foreach (var e in uiEntries)
            {
                e.target = tempTarget;

                e.MakeContent(scrollContent);


                e.DataToUI();
            }


        }


    }
}
