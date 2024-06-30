using HarmonyLib;
using LBoL.Core.Battle;
using LBoL.Presentation.UI.Panels;
using System;
using System.Collections.Generic;
using System.Text;

namespace LBoLEntitySideloader.UIAdditions.CardUIAdds
{
    [HarmonyPatch(typeof(CardUi), nameof(CardUi.EnterBattle))]
    class CardUi_EnterBattle_Patch
    {
        static void Postfix(CardUi __instance, BattleController battle)
        {
            battle.ActionViewer.Register(CardUiExtensions.AddCardsToExileViewer(__instance));
        }
    }


    [HarmonyPatch(typeof(CardUi), nameof(CardUi.LeaveBattle))]
    class CardUi_LeaveBattle_Patch
    {
        static void Postfix(CardUi __instance, BattleController battle)
        {
            battle.ActionViewer.Unregister(CardUiExtensions.AddCardsToExileViewer(__instance));
        }
    }
}
