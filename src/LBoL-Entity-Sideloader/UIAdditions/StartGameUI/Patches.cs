using HarmonyLib;
using LBoL.Core;
using LBoL.Presentation.UI.ExtraWidgets;
using LBoL.Presentation.UI.Panels;
using UnityEngine;

namespace LBoLEntitySideloader.UIAdditions.StartGameUI
{
    [HarmonyPatch(typeof(StartGamePanel))]
    class StartGameUI_Patches
    {
        [HarmonyPatch(nameof(StartGamePanel.RefreshConfirm)), HarmonyPostfix]
        static void UnlockCharacter(StartGamePanel __instance)
        {
            StartGamePanel p = __instance;

            if (p._playerIndex >= 0 && p._playerIndex <= p._players.Length &&
                null != p._players[p._playerIndex].Config.UnlockLevel)
            {
                p.characterHintRoot.gameObject.SetActive(false);
                p.characterHint.text = "";
                p.characterConfirmButton.interactable = true;
            }
        }

        [HarmonyPatch(nameof(StartGamePanel.RefreshDifficultyConfirm)), HarmonyPostfix]
        static void UnlockDifficulty(StartGamePanel __instance)
        {
            StartGamePanel p = __instance;

            if (!p.gameModeSwitch.IsOn)
            {
                p.difficultyGroups[p._difficultyIndex].SetLocked(false);
                p._isDifficultyLock = false;
                p.difficultyConfirmButton.interactable = true;
                p._confirmTooltip.SetDirect("");
            }
        }

        [HarmonyPatch(nameof(StartGamePanel.SetPuzzleStatus)), HarmonyPostfix]
        static void UnlockRequests(StartGamePanel __instance)
        {
            StartGamePanel p = __instance;

            for (int i = 0; i < PuzzleFlags.AllPuzzleFlags.Count; ++i)
            {
                foreach (PuzzleToggleWidget ptw in p._puzzleToggles.Values)
                {
                    ptw.IsLock = false;
                }
            }

            p.puzzleSelectAllGroup.SetActive(true);
        }

        [HarmonyPatch(nameof(StartGamePanel.SetJadeBoxStatus)), HarmonyPostfix]
        static void UnlockJadeboxes(StartGamePanel __instance)
        {
            StartGamePanel p = __instance;

            p.jadeBoxButton.interactable = true;
            p._jadeBoxTooltip.enabled = false;
            p.jadeBoxLockImage.gameObject.SetActive(false);
            p.jadeBoxText.color = Color.white;
        }
    }
}
