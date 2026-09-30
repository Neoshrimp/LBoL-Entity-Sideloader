using HarmonyLib;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Cards;
using LBoL.Presentation.UI.ExtraWidgets;
using LBoL.Presentation.UI.Panels;

namespace LBoLEntitySideloader.GameFixes
{
    /*
    a card in hand being auto-played (PlayCardAction, manual plays are UseCardActions) keeps
    its slot in the hand layout, as the play view takes the hand widget without re-laying out the rest
    this destroys the hand widget and re-layouts the hand before the play view runs, so the view makes
    a fresh widget for the card
    */
    [HarmonyPatch]
    internal static class AutoPlayHandFix
    {
        [HarmonyPatch(typeof(CardUi), nameof(CardUi.ViewPlayCard)), HarmonyPrefix]
        private static void ViewPlayCardPrefix(CardUi __instance, PlayCardAction action)
        {
            Card card = action.Args.Card;
            if (action.SourceZone != CardZone.Hand || card.IsPlayTwiceToken) return;

            HandCard hand = __instance.ExtractHandWidget(card);
            if (hand == null) return;

            UnityEngine.Object.Destroy(hand.gameObject);
            __instance.RefreshAll();
            __instance.AdjustCardsPosition(true);
            __instance.RefreshAllCardsEdge();
        }
    }
}
