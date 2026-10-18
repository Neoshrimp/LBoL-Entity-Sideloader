using HarmonyLib;
using LBoL.Presentation.I10N;
using LBoL.Presentation.InputSystemExtend;
using LBoL.Presentation.UI.ExtraWidgets;
using LBoL.Presentation.UI.Panels;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LBoLEntitySideloader.ModConfigs
{
    public static class ModOptionsButton
    {
        private const string ButtonName = "ModOptionsButton";
        private static readonly Vector3 ButtonPosition = new Vector3(5.5f, -15.15f, 10f);

        [HarmonyPatch(typeof(MainMenuPanel), "OnShowing")]
        private static class MainMenuPanel_OnShowing_Patch
        {
            private static void Postfix(MainMenuPanel __instance)
            {
                try { Inject(__instance.transform.Find("Main")); }
                catch (System.Exception ex) { Debug.LogError("[ModOptions] Failed to inject menu button: " + ex); }
            }
        }

        public static void Inject(Transform main)
        {
            if (main == null || main.Find(ButtonName) != null) return;

            var src = main.Find("MainMenuLayout/Database");
            if (src == null) { Debug.LogWarning("[ModOptions] Database button not found."); return; }

            var go = Object.Instantiate(src.gameObject, main);
            go.name = ButtonName;
            go.SetActive(true);
            go.transform.position = ButtonPosition;   // world position

            foreach (var c in go.GetComponentsInChildren<SimpleTooltipSource>(true)) Object.Destroy(c);
            foreach (var c in go.GetComponentsInChildren<GamepadNavigationOrigin>(true)) Object.Destroy(c);
            foreach (var c in go.GetComponentsInChildren<EventTrigger>(true)) Object.Destroy(c);

            foreach (var lt in go.GetComponentsInChildren<LocalizedText>(true)) lt.key = "";   // keep font switching, stop text overwrite
            go.AddComponent<ModOptionsButtonLabel>();                                          // sets text now and on every locale change

            var btn = go.GetComponent<Button>();
            ModOptionsPanel.Neutralize(btn.onClick);   // drop the copied "open Database" wiring
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(ModOptionsPanel.Open);
        }
    }

    public class ModOptionsButtonLabel : MonoBehaviour
    {
        private void OnEnable()
        {
            L10nManager.LocaleChanged += Refresh;
            Refresh();
            foreach (var tmp in GetComponentsInChildren<TextMeshProUGUI>(true)) tmp.alignment = TextAlignmentOptions.Right;
        }

        private void OnDisable() => L10nManager.LocaleChanged -= Refresh;

        private void Refresh()
        {
            var s = ModConfigPanelL10n.Translate("MainMenuButton");
            foreach (var tmp in GetComponentsInChildren<TextMeshProUGUI>(true)) tmp.text = s;
        }
    }
}