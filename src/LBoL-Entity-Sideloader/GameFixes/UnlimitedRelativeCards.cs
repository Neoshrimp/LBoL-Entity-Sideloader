using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using LBoL.Core.Cards;
using LBoL.Presentation.UI.Panels;
using LBoL.Presentation.UI.Widgets;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace LBoLEntitySideloader.GameFixes
{
    // past 10 bars the list scrolls (see RelativeCardsScroller)
    [HarmonyPatch(typeof(CardDetailPanel), "SetData")]
    internal static class UnlimitedRelativeCards
    {
        private static readonly AccessTools.FieldRef<CardDetailPanel, Transform> relativeCellLayout =
            AccessTools.FieldRefAccess<CardDetailPanel, Transform>("relativeCellLayout");
        private static readonly AccessTools.FieldRef<CardDetailPanel, List<CardWidget>> relativeCardWidgets =
            AccessTools.FieldRefAccess<CardDetailPanel, List<CardWidget>>("relativeCardWidgets");
        private static readonly AccessTools.FieldRef<CardDetailPanel, RecordCardCell> cardCellTemplate =
            AccessTools.FieldRefAccess<CardDetailPanel, RecordCardCell>("cardCellTemplate");

        private static void Postfix(CardDetailPanel __instance, Card card)
        {
            Transform layout = relativeCellLayout(__instance);
            if (card == null || layout == null) return;
            List<Card> cards = card.EnumerateRelativeCards().ToList();
            // bars only when the cards don't fit the full-size widgets
            int bars = cards.Count > relativeCardWidgets(__instance).Count ? cards.Count : 0;
            // vanilla stops at 10 bars, add the rest
            if (bars > 0)
            {
                foreach (Card extra in cards.Skip(10))
                {
                    RecordCardCell cell = Object.Instantiate(cardCellTemplate(__instance), layout);
                    cell.Card = extra;
                    cell.name = "RelativeCard:" + extra.Name;
                    cell.gameObject.SetActive(true);
                }
            }
            RelativeCardsScroller.For(layout).Restart(bars);
        }
    }

    // shows 10 bars at most, scrolls through the rest and loops back
    // hovering pauses it and allows wheel scrolling, it resumes 3 seconds after the pointer leaves
    internal sealed class RelativeCardsScroller : MonoBehaviour, IScrollHandler
    {
        private const int VisibleBars = 10;
        private const float HoldSeconds = 3f;
        private const float SecondsPerBar = 1f;
        private const float ResumeSeconds = 3f;
        // how fast wheel scrolling catches up
        private const float WheelSharpness = 15f;

        private enum Phase { HoldTop, Scroll, HoldBottom }

        private RectTransform rect;
        private GridLayoutGroup grid;
        private RectMask2D mask;
        private Image wheelArea;
        private Canvas canvas;
        private float baseHeight;

        private float step;
        private float overflow;
        private float offset;
        private float wheelTarget;

        private Phase phase;
        private float phaseTime;
        private float scrollFrom;
        private float scrollSeconds;

        private bool paused;
        private float unhoveredTime;

        public static RelativeCardsScroller For(Transform layout)
        {
            var scroller = layout.GetComponent<RelativeCardsScroller>();
            if (scroller != null) return scroller;
            scroller = layout.gameObject.AddComponent<RelativeCardsScroller>();
            scroller.rect = (RectTransform)layout;
            scroller.grid = layout.GetComponent<GridLayoutGroup>();
            scroller.mask = layout.gameObject.AddComponent<RectMask2D>();
            // invisible image so the wheel works in the gaps between bars
            scroller.wheelArea = layout.gameObject.AddComponent<Image>();
            scroller.wheelArea.color = Color.clear;
            scroller.baseHeight = scroller.rect.sizeDelta.y;
            return scroller;
        }

        public void Restart(int bars)
        {
            step = grid.cellSize.y + grid.spacing.y;
            int hidden = Mathf.Max(0, bars - VisibleBars);
            overflow = hidden * step;
            offset = wheelTarget = 0f;
            phase = Phase.HoldTop;
            phaseTime = 0f;
            paused = false;
            // sized to the visible bars, the mask clips to it
            mask.enabled = hidden > 0;
            wheelArea.enabled = hidden > 0;
            float height = hidden > 0 ? VisibleBars * step - grid.spacing.y : baseHeight;
            rect.sizeDelta = new Vector2(rect.sizeDelta.x, height);
            SetOffset(0f);
            // disabled, wheel scrolling goes to the panel
            enabled = hidden > 0;
        }

        // only called while enabled (bars overflow)
        public void OnScroll(PointerEventData eventData)
        {
            float ticks = eventData.currentInputModule != null
                ? eventData.currentInputModule.ConvertPointerEventScrollDeltaToTicks(eventData.scrollDelta).y
                : Mathf.Sign(eventData.scrollDelta.y);
            if (!paused) wheelTarget = offset;
            wheelTarget = Mathf.Clamp(wheelTarget - ticks * step, 0f, overflow);
            Pause();
        }

        private void Update()
        {
            // the panel uses unscaled time
            float dt = Time.unscaledDeltaTime;
            if (IsHovered())
                Pause();
            else if (paused && (unhoveredTime += dt) >= ResumeSeconds)
                Resume();

            if (paused)
                offset = Mathf.Lerp(offset, wheelTarget, 1f - Mathf.Exp(-WheelSharpness * dt));
            else
                Advance(dt);
            SetOffset(offset);
        }

        private void Advance(float dt)
        {
            phaseTime += dt;
            switch (phase)
            {
                case Phase.HoldTop:
                    if (phaseTime >= HoldSeconds) StartScroll();
                    break;
                case Phase.Scroll:
                    float progress = Mathf.Clamp01(phaseTime / scrollSeconds);
                    offset = Mathf.SmoothStep(scrollFrom, overflow, progress);
                    if (progress >= 1f) Enter(Phase.HoldBottom);
                    break;
                case Phase.HoldBottom:
                    if (phaseTime < HoldSeconds) break;
                    offset = 0f;
                    Enter(Phase.HoldTop);
                    break;
            }
        }

        private void Pause()
        {
            if (!paused) wheelTarget = offset;
            paused = true;
            unhoveredTime = 0f;
        }

        // continues scrolling down from where it was left
        private void Resume()
        {
            paused = false;
            offset = wheelTarget;
            if (offset >= overflow - 0.5f) Enter(Phase.HoldBottom);
            else StartScroll();
        }

        private void StartScroll()
        {
            scrollFrom = offset;
            scrollSeconds = (overflow - offset) / step * SecondsPerBar;
            Enter(Phase.Scroll);
        }

        private void Enter(Phase next)
        {
            phase = next;
            phaseTime = 0f;
        }

        // the bars fill the visible rect
        private bool IsHovered()
        {
            if (Mouse.current == null) return false;
            if (canvas == null) canvas = GetComponentInParent<Canvas>()?.rootCanvas;
            Camera cam = canvas == null || canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;
            return RectTransformUtility.RectangleContainsScreenPoint(rect, Mouse.current.position.ReadValue(), cam);
        }

        // negative top padding moves the bars up under the mask
        private void SetOffset(float value)
        {
            int top = -Mathf.RoundToInt(value);
            if (grid.padding.top == top) return;
            grid.padding.top = top;
            LayoutRebuilder.MarkLayoutForRebuild(rect);
        }
    }
}
