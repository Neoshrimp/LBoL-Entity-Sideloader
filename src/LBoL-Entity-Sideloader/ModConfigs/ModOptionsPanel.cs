using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using Cysharp.Threading.Tasks;
using LBoL.Base.Extensions;
using LBoL.Core;
using LBoL.Presentation;
using LBoL.Presentation.I10N;
using LBoL.Presentation.InputSystemExtend;
using LBoL.Presentation.UI;
using LBoL.Presentation.UI.ExtraWidgets;
using LBoL.Presentation.UI.Panels;
using LBoL.Presentation.UI.Widgets;
using LBoLEntitySideloader.Resource;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LBoLEntitySideloader.ModConfigs
{
    public class ModOptionsPanel : UiPanel, IInputActionHandler
    {
        // --- Static References & Constants ---
        /// <summary>
        /// Name of the panel
        /// </summary>
        private const string PanelName = "ModOptionsPanel";
        /// <summary>
        /// This thing
        /// </summary>
        public static ModOptionsPanel _instance;
        /// <summary>
        /// Path in the scene hierarchy to where the game's menus live
        /// </summary>
        private const string NormalLayerPath = "UICamera/Canvas/Root/NormalLayer";
        /// <summary>
        /// Path to the collection's card search
        /// </summary>
        private const string MuseumInputPath = "MuseumPanel/TabRoot/Cards/LeftScollView/Viewport/Content/TextFilter/TextFilterInput";
        /// <summary>
        /// Path relative to the settings panel, where the reset button lives.
        /// </summary>
        private const string ResetTemplatePath = "Root/Main/RightPanel/RecommendVolume";
        /// <summary>
        /// minimum height for each config row to prevent overlap.
        /// </summary>
        private const float MinRowHeight = 56f;
        /// <summary>
        /// Comma stuff for other languages (. vs , for decimals)
        /// </summary>
        private static readonly CultureInfo Inv = CultureInfo.InvariantCulture;

        // --- Cloned Base Game Elements (from ComplexRulesPanel) ---
        /// <summary>
        /// Root of the left tab where all the mod tabs live.
        /// </summary>
        public Transform leftRoot;
        /// <summary>
        /// Button prefab for each mod's name in the left tab.
        /// </summary>
        public GameObject leftTitleTemplate;
        /// <summary>
        /// scrollable content root area of the right tab, where all the configs are displayed.
        /// Its size has to be expanded whenever there are too many configs.
        /// </summary>
        public RectTransform rightContent;
        /// <summary>
        /// Template for text for fonts, colors, sizes, etc.
        /// </summary>
        public TextMeshProUGUI descriptionTextTemplate;
        /// <summary>
        /// Button outside the panel to close it when clicking it.
        /// </summary>
        public Button bgButton;

        // --- Internal UI References ---
        /// <summary>
        /// Controls interaction state of the panel (to disable it while capturing or closing animation)
        /// </summary>
        private CanvasGroup _canvasGroup;
        /// <summary>
        /// Vertical layout to contain all the rows of each config
        /// </summary>
        private RectTransform _currentPageConfigsContainer;
        /// <summary>
        /// Reference to the basegame settings panel
        /// </summary>
        private SettingPanel _settingsPanelReference;
        /// <summary>
        /// Reference to the input field from the base game's collection
        /// </summary>
        private TMP_InputField _inputFieldTemplate;
        /// <summary>
        /// Text displayed at the bottom, to be enabled, when a requiresRestart option is changed.
        /// </summary>
        private TextMeshProUGUI _restartText;
        /// <summary>
        /// Reference to the normal layer object where the game's menus live.
        /// </summary>
        private static Transform _normalLayer;
        /// <summary>
        /// Root where all the game's objects live.
        /// </summary>
        private static Transform NormalLayer
        {
            get
            {
                if (_normalLayer == null)
                {
                    var go = GameObject.Find(NormalLayerPath);
                    if (go != null) _normalLayer = go.transform;
                }
                return _normalLayer;
            }
        }

        // --- Data & Persistence State ---
        /// <summary>
        /// GuiD of the current plugin being displayed on the right side.
        /// </summary>
        private string _currentPluginGuid;
        /// <summary>
        /// List of all discovered plugins that have valid configs.
        /// </summary>
        private readonly List<Page> _pages = new List<Page>();
        /// <summary>
        /// List of modified config files until the panel closes.
        /// </summary>
        private readonly HashSet<ConfigFile> _dirtyConfigFiles = new HashSet<ConfigFile>();
        /// <summary>
        /// Single plugin's config page. Links the sidebar tab to the settings that that plugin has.
        /// </summary>
        private sealed class Page
        {
            /// <summary>
            /// Guid of the plugin
            /// </summary>
            public string Guid;
            /// <summary>
            /// Display name of the plugin.
            /// </summary>
            public string Title;
            /// <summary>
            /// All non-hidden configuration entries belonging to this plugin.
            /// </summary>
            public List<ConfigEntryBase> Entries;
        }
        /// <summary>
        /// Tracker for config entries marked with 'requiresRestart = true'.
        /// Determines whether the yellow "Restart required" banner should be displayed.
        /// </summary>
        internal static class RestartState
        {
            /// <summary>
            /// Stores the value of each modified entry prior to its first edit this session.
            /// Key: The config entry | Value: The stored value before any changes were made.
            /// </summary>
            private static readonly Dictionary<ConfigEntryBase, object> Original = new Dictionary<ConfigEntryBase, object>();

            /// <summary>
            /// First time the entry is to be changed this session, save the entry's initial value before it gets modified.
            /// </summary>
            /// <param name="e">The config entry about to be edited.</param>
            public static void Remember(ConfigEntryBase e)
            {
                if (!Original.ContainsKey(e)) Original[e] = e.BoxedValue;   // value before its first change
            }

            /// <summary>
            /// If any remembered value differs from its current one.
            /// </summary>
            public static bool Pending => Original.Any(kv => !Same(kv.Value, kv.Key.BoxedValue));

            /// <summary>
            /// Equality check that ignores small float precision differences (e.g., 0.100000001f vs 0.1f).
            /// </summary>
            private static bool Same(object a, object b)
            {
                if (a is float fa && b is float fb) return Math.Abs(fa - fb) < 1e-4f;
                if (a is double da && b is double db) return Math.Abs(da - db) < 1e-4;
                return Equals(a, b);
            }
        }


        #region Opening

        public static void Open()
        {
            if (_instance == null) _instance = FindExisting() ?? Create();
            if (_instance != null) _instance.Show();
        }

        private static ModOptionsPanel FindExisting()
        {
            var layer = NormalLayer;
            var t = layer != null ? layer.Find(PanelName) : null;
            return t != null ? t.GetComponent<ModOptionsPanel>() : null;
        }

        private static ModOptionsPanel Create()
        {
            var layer = NormalLayer;
            var tt = layer != null ? layer.Find("ComplexRulesPanel") : null;
            var template = tt != null ? tt.GetComponent<ComplexRulesPanel>() : null;
            if (template == null)
                template = Resources.FindObjectsOfTypeAll<ComplexRulesPanel>()
                                    .FirstOrDefault(p => p.gameObject.scene.IsValid());
            if (template == null) { Debug.LogWarning("[ModOptions] No ComplexRulesPanel found."); return null; }

            var holder = new GameObject("ModOptionsHolder");
            holder.SetActive(false);
            var clone = Instantiate(template.gameObject, holder.transform);
            clone.name = PanelName;

            var old = clone.GetComponent<ComplexRulesPanel>();
            var panel = clone.AddComponent<ModOptionsPanel>();

            const System.Reflection.BindingFlags F = System.Reflection.BindingFlags.Instance
                | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic
                | System.Reflection.BindingFlags.DeclaredOnly;
            for (var t = typeof(UiPanel); t != null && t != typeof(MonoBehaviour); t = t.BaseType)
                foreach (var f in t.GetFields(F))
                    if (!f.IsLiteral) f.SetValue(panel, f.GetValue(old));

            panel.leftRoot = old.titleRoot;
            panel.rightContent = old.rightContent;
            panel.descriptionTextTemplate = old.descriptionText;
            panel.leftTitleTemplate = old.titleTemplate;
            panel.bgButton = old.bgButton;

            // The clone inherits any cards/exhibits the Rules panel was showing when copied.
            old.entityContent.DestroyChildren();
            old.entityContent.gameObject.SetActive(false);

            DestroyImmediate(old);
            clone.transform.SetParent(template.transform.parent, false);
            Destroy(holder);
            return panel;
        }

        #endregion

        #region inherited stuff

        public void Awake()
        {
            bgButton.onClick.AddListener(Hide);
            rightContent.sizeDelta = Vector2.zero;
            leftTitleTemplate.SetActive(false);
            descriptionTextTemplate.gameObject.SetActive(false);
            _canvasGroup = GetComponent<CanvasGroup>();

            _currentPageConfigsContainer = NewGroup("SettingsList", rightContent, true, 18f, new RectOffset(40, 40, 20, 40));
            _currentPageConfigsContainer.anchorMin = new Vector2(0, 1);
            _currentPageConfigsContainer.anchorMax = new Vector2(1, 1);
            _currentPageConfigsContainer.pivot = new Vector2(0.5f, 1);
            _currentPageConfigsContainer.anchoredPosition = Vector2.zero;
            _currentPageConfigsContainer.sizeDelta = Vector2.zero;
            _currentPageConfigsContainer.gameObject.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            _restartText = MakeText(transform, ModConfigPanelL10n.Translate("RestartNotice"),
                            descriptionTextTemplate.fontSize, TextAlignmentOptions.Midline,
                            new Color(1f, 0.9f, 0.2f, 1f));
            _restartText.gameObject.AddComponent<LayoutElement>().ignoreLayout = true;
            var rrt = _restartText.rectTransform;
            rrt.anchorMin = rrt.anchorMax = new Vector2(0.5f, 0f);
            rrt.pivot = new Vector2(0.5f, 0f);
            rrt.sizeDelta = new Vector2(1400f, 60f);
            rrt.anchoredPosition = new Vector2(0f, 30f);
            _restartText.gameObject.SetActive(false);

            Application.quitting += FlushDirty;
        }
        protected override void OnShowing()
        {
            _canvasGroup.interactable = true;
            LoadPages();
            UiManager.PushActionHandler(this);
            RefreshRestartText();
        }

        private void Update()
        {
            if (_capApply == null) return;
            var input = UnityInput.Current;
            if (_capKeys == null)
                _capKeys = input.SupportedKeyCodes
                    .Except(new[] { KeyCode.None, KeyCode.Escape, KeyCode.Mouse0, KeyCode.Mouse1, KeyCode.Mouse2 })
                    .ToArray();

            foreach (var key in _capKeys)
            {
                if (!input.GetKeyUp(key)) continue;
                var mods = _capKeys.Where(input.GetKey).ToArray();
                var label = _capLabel; var read = _capRead; var apply = _capApply;
                EndCapture();
                apply(key, mods);
                if (label != null) label.text = read();
                return;
            }
        }

        protected override void OnHiding()
        {
            EndCapture();
            _canvasGroup.interactable = false;
            UiManager.PopActionHandler(this);
            FlushDirty();
        }

        void IInputActionHandler.OnCancel()
        {
            if (_capApply != null) { EndCapture(); return; }
            if (DropdownWidget.CurrentActiveDropdown != null)
            {
                DropdownWidget.CurrentActiveDropdown.dropdown.Hide();
                return;
            }
            if (Singleton<InputDeviceManager>.Instance.CurrentInputDevice == InputDeviceType.Gamepad)
                AudioManager.Button(1);
            Hide();
        }

        override protected void OnDestroy()
        {
            Application.quitting -= FlushDirty;   // static event, so unsubscribe or it leaks
            FlushDirty();
        }

        public override UniTask CustomLocalizationAsync() => UniTask.CompletedTask;
        public override void OnLocaleChanged()
        {
            ModConfigPanelL10n.ClearCache();
            RefreshRestartText();
            if (IsVisible) LoadPages();
        }
        #endregion

        #region Find Plugins
        private void LoadPages()
        {
            CollectPages();
            leftRoot.transform.DestroyChildren();

            for (int i = 0; i < _pages.Count; i++)
            {
                var page = _pages[i];
                var go = Instantiate(leftTitleTemplate, leftRoot);
                go.SetActive(true);
                go.GetComponentInChildren<TextMeshProUGUI>().text = PageTitle(page);
                go.GetComponent<Button>().onClick.AddListener(() => ShowPage(page));
                if (i == 0) go.AddComponent<GamepadNavigationOrigin>();
            }

            if (_pages.Count > 0) ShowPage(_pages[0]);
            else ClearList();
        }

        private void CollectPages()
        {
            _pages.Clear();

            // ScriptEngine never writes PluginInfo.Instance, so look the components up by GUID.
            var byGuid = new Dictionary<string, BaseUnityPlugin>();
            foreach (var p in Resources.FindObjectsOfTypeAll<BaseUnityPlugin>())
            {
                if (p == null || p.Info == null) continue;
                byGuid[p.Info.Metadata.GUID] = p;
            }

            foreach (var info in Chainloader.PluginInfos.Values.OrderBy(i => i.Metadata.Name))
            {
                var plugin = info.Instance;
                if (plugin == null) byGuid.TryGetValue(info.Metadata.GUID, out plugin);
                if (plugin == null) continue;

                var entries = plugin.Config
                    .Select(kv => kv.Value)
                    .Where(e => GetAttrs(e)?.Hidden != true)
                    .GroupBy(e => e.Definition.Section)
                    .OrderBy(g => string.IsNullOrEmpty(g.Key) ? 0 : 1)   // unsectioned first
                    .SelectMany(g => g)
                    .ToList();
                if (entries.Count > 0)
                    _pages.Add(new Page { Guid = info.Metadata.GUID, Title = info.Metadata.Name, Entries = entries });
            }
        }


        private static string PageTitle(Page p) =>
        ModConfigL10n.Get(p.Guid, "_Plugin", null, "Name") ?? p.Title;

        // "Name"/"Description"/"ButtonText"/"Choices"... for one entry. null if there is no yaml value.
        private string Loc(ConfigEntryBase e, string path1, params string[] extraPaths) =>
            ModConfigL10n.Get(_currentPluginGuid, e.Definition.Section, e.Definition.Key, path1, extraPaths);
        #endregion

        #region Build Page
        private void ShowPage(Page page)
        {
            EndCapture();
            ClearList();
            if (!EnsureTemplates()) return;
            _currentPluginGuid = page.Guid;

            string lastSection = null;
            MakeText(_currentPageConfigsContainer, PageTitle(page), descriptionTextTemplate.fontSize * 2f,
                     TextAlignmentOptions.Midline, null, FontStyles.Bold);
            foreach (var entry in page.Entries)
            {
                var section = entry.Definition.Section;
                if (section != lastSection)
                {
                    lastSection = section;
                    var sectionName = ModConfigL10n.Get(_currentPluginGuid, section, null, "_Name") ?? section;
                    if (!string.IsNullOrEmpty(sectionName))
                        MakeText(_currentPageConfigsContainer, sectionName, descriptionTextTemplate.fontSize * 1.6f,
                            TextAlignmentOptions.MidlineLeft, GlobalConfig.DefaultKeywordColor, FontStyles.Bold);
                }
                try { BuildRow(entry); }
                catch (Exception ex) { Debug.LogError($"[ModOptions] Failed to build {entry.Definition}: {ex}"); }
            }
            AddResetButton(page);
            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate(_currentPageConfigsContainer);
            LayoutRebuilder.ForceRebuildLayoutImmediate(_currentPageConfigsContainer);   // second pass: TMP wrapping needs a width first
            rightContent.sizeDelta = new Vector2(0f, _currentPageConfigsContainer.rect.height);

            var sr = rightContent.GetComponentInParent<ScrollRect>();
            if (sr != null) sr.verticalNormalizedPosition = 1f;
        }
        private void ClearList()
        {
            foreach (Transform c in _currentPageConfigsContainer)
            {
                c.gameObject.SetActive(false);   // Destroy is deferred; inactive children drop out of layout now
                Destroy(c.gameObject);
            }
        }

        private bool EnsureTemplates()
        {
            if (_settingsPanelReference == null)
                _settingsPanelReference = Resources.FindObjectsOfTypeAll<SettingPanel>().FirstOrDefault(p => p.gameObject.scene.IsValid());
            if (_inputFieldTemplate == null)
            {
                var layer = NormalLayer;
                var t = layer != null ? layer.Find(MuseumInputPath) : null;
                if (t != null) _inputFieldTemplate = t.GetComponentInChildren<TMP_InputField>(true);
            }
            if (_settingsPanelReference == null || _inputFieldTemplate == null)
            {
                Debug.LogWarning($"[ModOptions] Missing templates (SettingPanel: {_settingsPanelReference != null}, input: {_inputFieldTemplate != null}).");
                return false;
            }
            return true;
        }

        private void BuildRow(ConfigEntryBase e)
        {
            var row = NewGroup("Row", _currentPageConfigsContainer, true, 4f);
            var top = NewGroup("Top", row, false, 24f);

            var label = MakeText(top, Loc(e, "Name") ?? e.Definition.Key, descriptionTextTemplate.fontSize,
                         TextAlignmentOptions.MidlineLeft);

            label.textWrappingMode = TextWrappingModes.Normal;
            label.overflowMode = TextOverflowModes.Overflow;
            HalfWidth(label.gameObject);

            var slot = new GameObject("Slot", typeof(RectTransform)).GetComponent<RectTransform>();
            slot.SetParent(top, false);
            HalfWidth(slot.gameObject);

            float h = BuildControl(slot, e);
            if (slot.TryGetComponent<LayoutElement>(out var layout))
            {
                layout.minHeight = h;
            }
            else
            {
                slot.gameObject.AddComponent<LayoutElement>().minHeight = h;
            }

            var desc = Loc(e, "Description") ?? e.Description?.Description;
            if (!string.IsNullOrEmpty(desc))
                MakeText(row, desc, descriptionTextTemplate.fontSize * 0.75f, TextAlignmentOptions.TopLeft,
                         new Color(0.7f, 0.7f, 0.7f, 1f), usesRichText: true);
            if (GetAttrs(e)?.RequiresRestart == true)
                MakeText(row, ModConfigPanelL10n.Translate("RequiresRestart"), descriptionTextTemplate.fontSize * 0.75f,
                         TextAlignmentOptions.TopLeft, GlobalConfig.DefaultKeywordColor);
            if (GetAttrs(e)?.ReadOnly == true)
            {
                var cg = row.gameObject.AddComponent<CanvasGroup>();
                cg.interactable = false;
                cg.alpha = 0.5f;
            }
            AddSeparator(row);
        }


        private void AddResetButton(Page page)
        {
            var tplT = _settingsPanelReference.transform.Find(ResetTemplatePath);
            if (tplT == null) { Debug.LogWarning("[ModOptions] Reset template not found."); return; }
            var size = ((RectTransform)tplT).rect.size;

            var holder = new GameObject("ResetHolder", typeof(RectTransform), typeof(LayoutElement));
            holder.transform.SetParent(_currentPageConfigsContainer, false);

            float ResetTopGap = 100f;
            float ResetBottomGap = 100f;   // room for the scroller and the restart notice

            var go = Instantiate(tplT.gameObject, holder.transform);
            StripExtras(go);
            go.SetActive(true);
            var rt = (RectTransform)go.transform;
            rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0f);
            rt.pivot = new Vector2(0.5f, 0f);
            rt.sizeDelta = size;

            foreach (var lt in go.GetComponentsInChildren<LocalizedText>(true)) lt.enabled = false;
            var common = go.transform.Find("CommonButton");
            var root = common != null ? common : go.transform;
            foreach (var tmp in root.GetComponentsInChildren<TextMeshProUGUI>(true)) tmp.text = ModConfigPanelL10n.Translate("Reset");

            foreach (var b in go.GetComponentsInChildren<Button>(true)) { Neutralize(b.onClick); b.onClick.RemoveAllListeners(); }
            var btn = root.GetComponentInChildren<Button>(true) ?? go.GetComponentInChildren<Button>(true);
            if (btn != null) btn.onClick.AddListener(() => ResetPage(page));

            var bounds = MeasureTree(rt);
            rt.anchoredPosition = new Vector2(0f, ResetBottomGap - bounds.min.y);   // visual bottom edge sits at the gap
            holder.GetComponent<LayoutElement>().minHeight = bounds.size.y + ResetTopGap + ResetBottomGap;
        }
        private void ResetPage(Page page)
        {
            foreach (var e in page.Entries)
            {
                if (GetAttrs(e)?.ReadOnly == true) continue;
                SetValue(e, e.DefaultValue);
            }
            ShowPage(page);   // rebuild so every widget re-reads its value
        }

        private void AddSeparator(RectTransform row)
        {
            var lineTpl = _settingsPanelReference.actionMappingRowTemplate.transform.Find("Line");
            if (lineTpl == null) return;
            var r = ((RectTransform)lineTpl).rect;
            float thickness = Mathf.Clamp(Mathf.Min(r.width, r.height), 1f, 6f); // works if the original is drawn vertically

            var go = Instantiate(lineTpl.gameObject, row);
            go.name = "Separator";
            go.SetActive(true);
            go.AddComponent<LayoutElement>().ignoreLayout = true;
            var rt = (RectTransform)go.transform;
            rt.anchorMin = new Vector2(0, 0);
            rt.anchorMax = new Vector2(1, 0);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = new Vector2(0f, thickness);
            rt.anchoredPosition = new Vector2(0f, -9f);   // half of the 18px list spacing
        }

        #region Widget Builders
        private float BuildControl(RectTransform slot, ConfigEntryBase e)
        {
            if (GetAttrs(e)?.OnClick is Action click) return BuildButton(slot, e, click);
            var t = e.SettingType;
            if (t == typeof(bool)) return BuildSwitch(slot, e);
            // KeyCode is an enum, so this must come before the dropdown check
            if (t == typeof(KeyboardShortcut) || t == typeof(KeyCode)) return BuildKey(slot, e);
            if (IsNumeric(t) && TryGetRange(e, out var min, out var max)) return BuildSlider(slot, e, min, max);
            var choices = GetChoices(e);
            if (choices != null) return BuildDropdown(slot, e, choices);
            return BuildInput(slot, e);
        }

        private float BuildSwitch(RectTransform slot, ConfigEntryBase e)
        {
            var tpl = _settingsPanelReference.keywordSwitch;                 // SwitchWidget is on the "Switch" child
            var size = TemplateSize(tpl);
            var sw = Instantiate(tpl, slot);
            StripExtras(sw.gameObject);
            sw.gameObject.SetActive(true);
            Neutralize(sw.onToggleChanged);
            SetFixedText(sw.textOn, ModConfigPanelL10n.Translate("On"));
            SetFixedText(sw.textOff, ModConfigPanelL10n.Translate("Off"));
            PlaceRight((RectTransform)sw.transform, size, ControlRightInset);
            sw.SetValueWithoutNotifier((bool)e.BoxedValue);
            sw.AddListener(v => SetValue(e, v));
            return Mathf.Max(MinRowHeight, size.y);
        }

        private static void SetFixedText(TextMeshProUGUI t, string s)
        {
            var lt = t.GetComponent<LocalizedText>();
            if (lt != null) lt.enabled = false;         // otherwise it re-localizes over our text
            t.text = s;
        }

        private float BuildSlider(RectTransform slot, ConfigEntryBase e, double min, double max)
        {
            var tpl = _settingsPanelReference.bgmVolumeSlider;
            var size = TemplateSize(tpl);
            float h = Mathf.Max(MinRowHeight, size.y);

            bool whole = e.SettingType != typeof(float) && e.SettingType != typeof(double);
            Func<float, string> fmt = v => whole ? Mathf.RoundToInt(v).ToString(Inv) : v.ToString("0.##", Inv);
            float cur = Convert.ToSingle(e.BoxedValue, Inv);

            var valueText = MakeText(slot, "", descriptionTextTemplate.fontSize, TextAlignmentOptions.MidlineLeft);
            valueText.enableWordWrapping = false;
            valueText.overflowMode = TextOverflowModes.Overflow;
            // Size numbers to fit 8 digits.
            float w = valueText.GetPreferredValues("-888888").x + 16f;

            var vrt = valueText.rectTransform;
            vrt.anchorMin = new Vector2(1, 0);
            vrt.anchorMax = new Vector2(1, 1);
            vrt.pivot = new Vector2(1, 0.5f);
            vrt.sizeDelta = new Vector2(w, 0f);
            vrt.anchoredPosition = Vector2.zero;

            var sl = Instantiate(tpl, slot);
            StripExtras(sl.gameObject);
            sl.gameObject.SetActive(true);
            Neutralize(sl.onValueChanged);
            PlaceStretch((RectTransform)sl.transform, h, w + 10f);

            sl.wholeNumbers = whole;
            sl.minValue = (float)min;
            sl.maxValue = (float)max;
            sl.SetValueWithoutNotify(cur);
            valueText.text = fmt(sl.value);
            sl.onValueChanged.AddListener(v =>
            {
                valueText.text = fmt(v);
                SetValue(e, Convert.ChangeType(v, e.SettingType, Inv));
            });
            return h;
        }

        private static bool TryGetRange(ConfigEntryBase e, out double min, out double max)
        {
            min = max = 0;
            var av = e.Description?.AcceptableValues;
            if (av == null) return false;
            var t = av.GetType();
            var minP = t.GetProperty("MinValue");
            var maxP = t.GetProperty("MaxValue");
            if (minP == null || maxP == null) return false;
            min = Convert.ToDouble(minP.GetValue(av, null), Inv);
            max = Convert.ToDouble(maxP.GetValue(av, null), Inv);
            return true;
        }

        private float BuildDropdown(RectTransform slot, ConfigEntryBase e, List<object> choices)
        {
            var tpl = _settingsPanelReference.frameRateDropdown;             // DropdownWidget is on the ROW root here
            float h = Mathf.Max(MinRowHeight, TemplateSize(tpl).y,
                                ((RectTransform)tpl.dropdown.transform).rect.height);
            var dd = Instantiate(tpl, slot);
            StripExtras(dd.gameObject);
            dd.gameObject.SetActive(true);

            // Remove the cloned title. Deactivate first because Destroy is deferred and the layout would still see it.
            var key = dd.transform.Find("KeyTmp");
            if (key != null) { key.gameObject.SetActive(false); Destroy(key.gameObject); }

            Neutralize(dd.dropdown.onValueChanged);
            PlaceStretch((RectTransform)dd.transform, h, ControlRightInset);

            // The inner dropdown was positioned beside the title; make it fill the slot instead
            var inner = (RectTransform)dd.dropdown.transform;
            inner.anchorMin = Vector2.zero;
            inner.anchorMax = Vector2.one;
            inner.offsetMin = inner.offsetMax = Vector2.zero;

            dd.dropdown.ClearOptions();
            dd.dropdown.AddOptions(choices.Select(c =>
                Loc(e, "Choices", c.ToString()) ?? c.ToString()).ToList());
            dd.dropdown.SetValueWithoutNotify(Mathf.Max(0, choices.FindIndex(c => Equals(c, e.BoxedValue))));
            dd.dropdown.RefreshShownValue();
            dd.dropdown.onValueChanged.AddListener(i => SetValue(e, choices[i]));
            return h;
        }

        private static List<object> GetChoices(ConfigEntryBase e)
        {
            var av = e.Description?.AcceptableValues;
            var listP = av?.GetType().GetProperty("AcceptableValues");
            if (listP != null)
                return ((IEnumerable)listP.GetValue(av, null)).Cast<object>().ToList();
            var t = e.SettingType;
            if (t.IsEnum && !t.IsDefined(typeof(FlagsAttribute), false))
                return Enum.GetValues(t).Cast<object>().ToList();
            return null;
        }

        // ---- string / unbounded numbers / anything else
        private static float NumberBoxWidth = 320f;
        private const float InputFontScale = 1.3f;
        private float BuildInput(RectTransform slot, ConfigEntryBase e)
        {
            var size = ((RectTransform)_inputFieldTemplate.transform).rect.size;
            var inp = Instantiate(_inputFieldTemplate, slot);
            StripExtras(inp.gameObject);
            inp.gameObject.SetActive(true);
            if (inp.textComponent != null)
            {
                inp.textComponent.enableAutoSizing = false;
                inp.textComponent.fontSize *= InputFontScale;
            }
            if (inp.placeholder is TMP_Text ph)
            {
                ph.enableAutoSizing = false;
                ph.fontSize *= InputFontScale;
            }
            inp.characterLimit = 0;
            Neutralize(inp.onValueChanged);
            Neutralize(inp.onEndEdit);
            Neutralize(inp.onSubmit);
            Neutralize(inp.onSelect);
            Neutralize(inp.onDeselect);

            var t = e.SettingType;
            bool isString = t == typeof(string);
            bool numeric = IsNumeric(t);
            float h = Mathf.Max(MinRowHeight, size.y);

            if (numeric) PlaceRight((RectTransform)inp.transform, new Vector2(NumberBoxWidth, h), ControlRightInset);
            else PlaceStretch((RectTransform)inp.transform, h, ControlRightInset);

            inp.contentType = t == typeof(int) || t == typeof(long) ? TMP_InputField.ContentType.IntegerNumber
                            : t == typeof(float) || t == typeof(double) ? TMP_InputField.ContentType.DecimalNumber
                            : TMP_InputField.ContentType.Standard;

            Func<string> read = () => isString
                ? (string)e.BoxedValue ?? ""
                : TomlTypeConverter.ConvertToString(e.BoxedValue, t);

            inp.SetTextWithoutNotify(read());
            inp.onEndEdit.AddListener(s =>
            {
                try { SetValue(e, isString ? s : TomlTypeConverter.ConvertToValue(s, t)); }
                catch (Exception ex) { Debug.LogWarning($"[ModOptions] Rejected '{s}' for {e.Definition}: {ex.Message}"); }
                inp.SetTextWithoutNotify(read());
            });
            return h;
        }

        private float BuildButton(RectTransform slot, ConfigEntryBase e, Action click)
        {
            var tplT = _settingsPanelReference.transform.Find(ResetTemplatePath);
            if (tplT == null) return MinRowHeight;
            var size = ((RectTransform)tplT).rect.size;
            var go = Instantiate(tplT.gameObject, slot);
            StripExtras(go);
            go.SetActive(true);
            var rt = (RectTransform)go.transform;
            PlaceRight(rt, size);

            foreach (var lt in go.GetComponentsInChildren<LocalizedText>(true)) lt.enabled = false;
            string label = Loc(e, "ButtonText");
            if (string.IsNullOrEmpty(label)) label = GetAttrs(e)?.ButtonText;
            if (string.IsNullOrEmpty(label)) label = Loc(e, "Name");
            if (string.IsNullOrEmpty(label)) label = e.Definition.Key;

            float textW = 0f;
            foreach (var tmp in go.GetComponentsInChildren<TextMeshProUGUI>(true))
            {
                tmp.text = label;
                tmp.textWrappingMode = TextWrappingModes.NoWrap;
                tmp.overflowMode = TextOverflowModes.Overflow;
                textW = Mathf.Max(textW, tmp.GetPreferredValues(label).x);
            }

            foreach (var b in go.GetComponentsInChildren<Button>(true)) { Neutralize(b.onClick); b.onClick.RemoveAllListeners(); }
            var btn = go.GetComponentInChildren<Button>(true);
            if (btn != null) btn.onClick.AddListener(() =>
            {
                try { click(); } catch (Exception ex) { Debug.LogError("[ModOptions] Button handler failed: " + ex); }
            });

            // Widen the art if the label doesn't fit
            var art = btn != null ? (RectTransform)btn.transform : rt;
            const float ButtonTextMargin = 40f;                 // space left and right of the text
            float artW = RectTransformUtility.CalculateRelativeRectTransformBounds(rt, art).size.x;
            float needed = textW + ButtonTextMargin;
            if (needed > artW)
                art.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, art.rect.width + (needed - artW));

            // Put the art's right edge at the shared inset, whatever the template padding is
            var ab = RectTransformUtility.CalculateRelativeRectTransformBounds(rt, art);
            rt.anchoredPosition = new Vector2(-ControlRightInset - ab.max.x, 0f);

            var bounds = MeasureTree(rt);
            return Mathf.Max(MinRowHeight, bounds.size.y + 8f);
        }

        #region Key Building and capturing
        private TextMeshProUGUI _capLabel;
        private Func<string> _capRead;
        private Action<KeyCode, KeyCode[]> _capApply;
        private KeyCode[] _capKeys;
        private float BuildKey(RectTransform slot, ConfigEntryBase e)
        {
            var tpl = _settingsPanelReference.actionMappingRowTemplate;      // inactive template, so the clone's Awake runs on SetActive(true)
            var size = TemplateSize(tpl);
            var row = Instantiate(tpl, slot);
            StripExtras(row.gameObject);
            row.actionNameText.gameObject.SetActive(false);   // our own label is on the left
            row.gameObject.SetActive(true);
            Neutralize(row.keyboardButton.onClick);
            PlaceStretch((RectTransform)row.transform, Mathf.Max(MinRowHeight, size.y), 0f);

            var btn = (RectTransform)row.keyboardButton.transform;
            btn.anchorMin = Vector2.zero;
            btn.anchorMax = Vector2.one;
            btn.offsetMin = btn.offsetMax = Vector2.zero;

            bool isShortcut = e.SettingType == typeof(KeyboardShortcut);
            Func<string> read = () => e.BoxedValue.ToString();
            var label = row.keyboardButtonText;
            label.text = read();

            row.ButtonClicked += () => BeginCapture(label, read, (key, mods) =>
                SetValue(e, isShortcut ? new KeyboardShortcut(key, mods) : key));

            return Mathf.Max(MinRowHeight, size.y);
        }

        private void BeginCapture(TextMeshProUGUI label, Func<string> read, Action<KeyCode, KeyCode[]> apply)
        {
            EndCapture();
            _capLabel = label; _capRead = read; _capApply = apply;
            label.text = ModConfigPanelL10n.Translate("PressAKey");
            _canvasGroup.interactable = false;
        }

        private void EndCapture()
        {
            if (_capApply == null) return;
            var label = _capLabel; var read = _capRead;
            _capApply = null; _capLabel = null; _capRead = null;
            _canvasGroup.interactable = true;
            if (label != null) label.text = read();
        }
        #endregion

        #endregion

        #endregion

        #region Data updates and validation
        private void SetValue(ConfigEntryBase e, object v)
        {
            if (GetAttrs(e)?.RequiresRestart == true) RestartState.Remember(e);
            var f = e.ConfigFile;
            bool prev = f.SaveOnConfigSet;
            f.SaveOnConfigSet = false;               // defer disk write; flushed in OnHiding
            try { e.BoxedValue = v; }
            finally { f.SaveOnConfigSet = prev; }
            if (prev) _dirtyConfigFiles.Add(f);
            RefreshRestartText();
        }
        private void FlushDirty()
        {
            foreach (var f in _dirtyConfigFiles)
            {
                try { f.Save(); }
                catch (Exception ex) { Debug.LogError($"[ModOptions] Failed to save config: {ex.Message}"); }
            }
            _dirtyConfigFiles.Clear();
        }

        private void RefreshRestartText()
        {
            if (_restartText == null) return;
            _restartText.text = ModConfigPanelL10n.Translate("RestartNotice");   // use your renamed class
            _restartText.font = descriptionTextTemplate.font;
            _restartText.fontSharedMaterial = descriptionTextTemplate.fontSharedMaterial;
            _restartText.gameObject.SetActive(RestartState.Pending);
            _restartText.transform.SetAsLastSibling();
        }
        private static ConfigurationManagerAttributes GetAttrs(ConfigEntryBase e)
        {
            var tags = e.Description?.Tags;
            if (tags == null) return null;
            foreach (var t in tags)
                if (t is ConfigurationManagerAttributes a) return a;
            return null;
        }

        private static bool IsNumeric(Type t) =>
            t == typeof(int) || t == typeof(float) || t == typeof(double) || t == typeof(long);


        #endregion

        #region UI Helpers
        private static RectTransform NewGroup(string name, Transform parent, bool vertical, float spacing,
                                              RectOffset padding = null)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            HorizontalOrVerticalLayoutGroup g = vertical
                ? go.AddComponent<VerticalLayoutGroup>()
                : go.AddComponent<HorizontalLayoutGroup>();
            g.spacing = spacing;
            g.childControlWidth = true;
            g.childControlHeight = true;
            g.childForceExpandWidth = true;
            g.childForceExpandHeight = false;
            g.childAlignment = vertical ? TextAnchor.UpperLeft : TextAnchor.MiddleLeft;
            if (padding != null) g.padding = padding;
            return (RectTransform)go.transform;
        }

        private TextMeshProUGUI MakeText(Transform parent, string text, float size, TextAlignmentOptions align,
                                         Color? color = null, FontStyles style = FontStyles.Normal, bool usesRichText = false)
        {
            var go = new GameObject("Text", typeof(RectTransform));
            go.transform.SetParent(parent, false);
            var t = go.AddComponent<TextMeshProUGUI>();
            t.font = descriptionTextTemplate.font;
            t.fontSharedMaterial = descriptionTextTemplate.fontSharedMaterial;
            t.fontSize = size;
            t.alignment = align;
            t.fontStyle = style;
            t.color = color ?? descriptionTextTemplate.color;
            t.richText = usesRichText;          // config text is data, not markup
            t.raycastTarget = false;
            t.text = text;
            return t;
        }

        private static void HalfWidth(GameObject go)
        {
            var le = go.GetComponent<LayoutElement>() ?? go.AddComponent<LayoutElement>();
            le.minWidth = 0f;
            le.preferredWidth = 0f;
            le.flexibleWidth = 1f;
        }
        private const float ControlRightInset = 100f;   // tune until it lines up with the button

        private static void PlaceRight(RectTransform rt, Vector2 size, float rightInset = 0f)
        {
            rt.anchorMin = rt.anchorMax = new Vector2(1, 0.5f);
            rt.pivot = new Vector2(1, 0.5f);
            rt.sizeDelta = size;
            rt.anchoredPosition = new Vector2(-rightInset, 0f);
        }

        private static void PlaceStretch(RectTransform rt, float height, float rightInset)
        {
            rt.anchorMin = new Vector2(0, 0.5f);
            rt.anchorMax = new Vector2(1, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = new Vector2(-rightInset, height);
            rt.anchoredPosition = new Vector2(-rightInset / 2f, 0f);
        }

        private static void StripExtras(GameObject go)
        {
            foreach (var c in go.GetComponentsInChildren<SimpleTooltipSource>(true)) Destroy(c);
            foreach (var c in go.GetComponentsInChildren<EventTrigger>(true)) Destroy(c);
        }

        public static void Neutralize(UnityEventBase ev)
        {
            for (int i = 0; i < ev.GetPersistentEventCount(); i++)
                ev.SetPersistentListenerState(i, UnityEventCallState.Off);
        }

        
        static Bounds MeasureTree(RectTransform root) =>
        RectTransformUtility.CalculateRelativeRectTransformBounds(root, root);

        private static Vector2 TemplateSize(Component c) => ((RectTransform)c.transform).rect.size;
        
        #endregion

    }
}