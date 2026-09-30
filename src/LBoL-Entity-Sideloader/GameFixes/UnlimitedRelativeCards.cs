using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
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

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return new CodeMatcher(instructions)
                // if (list.Count > 10) { list = list.Take(10).ToList(); LogWarning(...); }
                .MatchStartForward(
                    new CodeMatch(ci => ci.opcode.Name.StartsWith("callvirt") && ci.operand?.ToString().Contains("get_Count") == true),
                    new CodeMatch(ci => ci.LoadsConstant(10)),
                    new CodeMatch(ci => ci.opcode.Name.StartsWith("ble")))
                .ThrowIfInvalid("CardDetailPanel.SetData: relative card cap not found")
                .Advance(1)
                .SetInstruction(new CodeInstruction(OpCodes.Ldc_I4, int.MaxValue))
                .InstructionEnumeration();
        }

        private static void Postfix(CardDetailPanel __instance, Card card)
        {
            Transform layout = relativeCellLayout(__instance);
            if (card == null || layout == null) return;
            // Bars are only used when the cards do not fit the full-size widgets.
            int count = card.EnumerateRelativeCards().Count();
            int bars = count > relativeCardWidgets(__instance).Count ? count : 0;
            RelativeCardsScroller.For(layout).Restart(bars);
        }
    }

    // Shows at most 10 bars. With more, it holds on the first 10, scrolls down until the last
    // bar is at the bottom, holds there, then cuts back to the first 10 and repeats.
    // Hovering the bars pauses it and lets the wheel scroll them; 3 seconds after the pointer
    // leaves, it eases back into scrolling down from where it is.
    internal sealed class RelativeCardsScroller : MonoBehaviour, IScrollHandler
    {
        private const int VisibleBars = 10;
        private const float HoldSeconds = 3f;
        private const float SecondsPerBar = 1f;
        private const float ResumeSeconds = 3f;
        // How quickly a wheel scroll catches up with where it was scrolled to.
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
            // A bar's visuals are smaller than its cell, so raycasts between bars would miss
            // them; this invisible image catches the wheel anywhere in the box, behind the bars.
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
            // The mask clips to this rect, so it is sized to exactly the visible bars.
            mask.enabled = hidden > 0;
            wheelArea.enabled = hidden > 0;
            float height = hidden > 0 ? VisibleBars * step - grid.spacing.y : baseHeight;
            rect.sizeDelta = new Vector2(rect.sizeDelta.x, height);
            SetOffset(0f);
            // Disabled, it also leaves wheel scrolling to the panel.
            enabled = hidden > 0;
        }

        // Only reaches here while enabled, i.e. while the bars overflow.
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
            // The panel runs on unscaled time (its tweens ignore the time scale).
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

        // Eases into scrolling down from wherever it was left.
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

        // The bars fill the whole visible rect, so the pointer being in it is a bar hovered.
        private bool IsHovered()
        {
            if (Mouse.current == null) return false;
            if (canvas == null) canvas = GetComponentInParent<Canvas>()?.rootCanvas;
            Camera cam = canvas == null || canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;
            return RectTransformUtility.RectangleContainsScreenPoint(rect, Mouse.current.position.ReadValue(), cam);
        }

        // Negative top padding moves the bars up while the mask stays put.
        private void SetOffset(float value)
        {
            int top = -Mathf.RoundToInt(value);
            if (grid.padding.top == top) return;
            grid.padding.top = top;
            LayoutRebuilder.MarkLayoutForRebuild(rect);
        }
    }
}
