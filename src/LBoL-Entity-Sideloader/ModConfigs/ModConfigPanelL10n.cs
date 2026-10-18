using LBoL.Core;
using LBoLEntitySideloader.Resource;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using YamlDotNet.RepresentationModel;

namespace LBoLEntitySideloader.ModConfigs
{
    /// <summary>
    /// The option panel's localization.
    /// </summary>
    public static class ModConfigPanelL10n
    {
        const string RootYamlKey = "ModOptionsPanel";
        const string yamlFileName = "ConfigPanel";
        private static LocalizationFiles _files;
        private static readonly Dictionary<Locale, Dictionary<string, string>> _cache =
            new Dictionary<Locale, Dictionary<string, string>>();

        /// <summary>
        /// Last-resort English, so the menu works even with no yaml at all.
        /// </summary>
        private static readonly Dictionary<string, string> Defaults = new Dictionary<string, string>
        {
            ["MainMenuButton"] = "Mod Configuration",
            ["Reset"] = "Reset",
            ["RequiresRestart"] = "Requires Restart",
            ["RestartNotice"] = "Some changes require restarting the game to take effect.",
            ["PressAKey"] = "Press a key...",
            ["On"] = "On",
            ["Off"] = "Off",
        };

        
        /// <summary>
        /// Initializes the localization for the panel. Called from awake.
        /// </summary>
        /// <param name="source">directory source</param>
        public static void Init(IResourceSource source)
        {
            _files = new LocalizationFiles(source, Locale.En);
            _files.DiscoverAndLoadLocFiles(yamlFileName);   // finds ConfigPanelEn.yaml, ConfigPanelZhHans.yaml, ...
            _cache.Clear();
        }

        public static void ClearCache() => _cache.Clear();

        /// <summary>
        /// Translates a key.
        /// Priority: current locale yaml->english yaml->defaults above
        /// </summary>
        public static string Translate(string key)
        {
            if (GetOrLoadTable(Localization.CurrentLocale).TryGetValue(key, out var s)) return s;
            if (GetOrLoadTable(Locale.En).TryGetValue(key, out s)) return s;
            return Defaults.TryGetValue(key, out s) ? s : key;
        }

        /// <summary>
        /// Replaces {0}, {1} in text with the args. Currently unused.
        /// </summary>
        public static string Format(string key, params object[] args) => string.Format(Translate(key), args);

        private static Dictionary<string, string> GetOrLoadTable(Locale locale)
        {
            if (_cache.TryGetValue(locale, out var table)) return table;
            table = new Dictionary<string, string>();
            _cache[locale] = table;

            // Ensure a file actually exists for this locale to avoid accidental premature fallback
            if (_files == null || !_files.fileNames.ContainsKey(locale)) return table;
            try
            {
                var root = _files.Load(locale);
                if (root != null
                    && root.Children.TryGetValue(RootYamlKey, out var node)
                    && node is YamlMappingNode map)
                {
                    foreach (var kvPair in map.Children)
                        if (kvPair.Key is YamlScalarNode key && kvPair.Value is YamlScalarNode value)
                            table[key.Value] = value.Value;
                }
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogWarning($"[ModOptions] Failed to load {locale} localization: {ex.Message}");
            }
            return table;
        }
    }

    /// <summary>
    /// The localization of the particular configs of each mod. <br></br>
    /// For a mod author, only the <see cref="Register(string, IResourceSource, string)"/> method is relevant to you.
    /// In addition, see <see cref="LBoLEntitySideloader.ModConfigs.ConfigurationManagerAttributes"/> for extra options for your configs.
    /// </summary>
    public static class ModConfigL10n
    {
        /// <summary>
        /// Holds the raw files and parsed yaml trees for a single plugin
        /// </summary>
        private sealed class PluginLocalizationSource
        {
            public LocalizationFiles Files;
            public Dictionary<Locale, YamlMappingNode> Roots = new Dictionary<Locale, YamlMappingNode>();
        }
        /// <summary>
        /// Plugin GUID -> Plugin Localization Source.
        /// </summary>
        private static readonly Dictionary<string, PluginLocalizationSource> _pluginSources = new();

        /// <summary>
        /// Registers a mod's localization files. Call once from your mod's Awake.
        /// If never called, the panel falls back to the hardcoded strings from ConfigFile.Bind().
        /// <para>ModConfig yaml structure looks like this:</para>
        /// <code>
        /// _Plugin:
        ///   Name: Your mod name
        /// # ^above is Optional
        ///   
        /// Section1:
        ///   _Name: Section name
        ///   setting1:
        ///     Name: setting 1
        ///     Description: this is setting 1
        ///   setting2:
        ///     Name: setting 2
        ///     Description: this is setting 2
        ///     
        /// Section2Dropdowns:
        ///   _Name: Dropdowns Section
        ///   enumSettingKey:
        ///     Name: Mode
        ///     Description: Select a mode.
        ///     Choices:
        ///       OptionA: Option A Display Text
        ///       OptionB: Option B Display Text
        /// </code>
        /// </summary>
        /// <param name="pluginGuid">your plugin guid. From Pinfo/PluginInfo class.</param>
        /// <param name="source">Source where the yamls reside (probably directory source)</param>
        /// <param name="prefix">base name of the yaml file. E.G. ModConfigsEn.yaml, ModConfigsZhHans.yaml.</param>
        public static void Register(string pluginGuid, IResourceSource source, string prefix = "ModConfigs")
        {
            var f = new LocalizationFiles(source, Locale.En);
            f.DiscoverAndLoadLocFiles(prefix);
            _pluginSources[pluginGuid] = new PluginLocalizationSource { Files = f };
        }

        /// <summary>
        /// Retrieves or parses the root YAML mapping node for a given plugin and locale.
        /// </summary>
        private static YamlMappingNode GetOrLoadRootNode(PluginLocalizationSource source, Locale locale)
        {
            if (source.Roots.TryGetValue(locale, out var rootNode)) return rootNode;
            try { rootNode = source.Files.fileNames.ContainsKey(locale) ? source.Files.Load(locale) : null; }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogWarning($"[ModOptions] Bad yaml for {locale}: {ex.Message}");
                rootNode = null;
            }
            return source.Roots[locale] = rootNode;
        }

        /// <summary>
        /// Safely gets a child node from a parent node by key.
        /// <code>
        /// parentNode:
        ///   Key: (- gets this)
        ///   otherKey:
        /// </code>
        /// </summary>
        private static YamlNode GetChildNode(YamlNode parentNode, string Key) =>
            parentNode is YamlMappingNode m && m.Children.TryGetValue(Key, out var c) ? c : null;


        /// <summary>
        /// Traverses a mod's YAML localization tree to find a translated string.<br/>
        /// Hierarchy:
        /// <code>
        /// Section:
        ///   ConfigKey:
        ///     path1: (value here if no extrapaths)
        ///       extraPath1:
        ///         extraPath2: value
        /// </code>
        /// </summary>
        /// <param name="guid">The mod's GUID.</param>
        /// <param name="section">The top level section of each yaml. Such as _Plugin or your config's sections.</param>
        /// <param name="configKey">One level deeper. Used for each config within a section. Pass <c>null</c> to stay at the section level.</param>
        /// <param name="path1">The property to read (e.g., Name, Description) or first subpath.</param>
        /// <param name="extraPaths">Optional extra keys to drill deeper into the tree (e.g., dropdown choice values).</param>
        /// <returns>The translated string if found; otherwise, null.</returns>
        public static string Get(string guid, string section, string configKey, string path1, params string[] extraPaths)
        {
            if (guid == null || !_pluginSources.TryGetValue(guid, out var source))
                return null;

            foreach (var locale in new[] { Localization.CurrentLocale, Locale.En })
            {
                var root = GetOrLoadRootNode(source, locale);
                if (root == null) continue;

                YamlNode currentNode = GetChildNode(root, section);
                if (configKey != null)
                    currentNode = GetChildNode(currentNode, configKey);

                // First step: drill into the mandatory property (e.g., "Name", "Description", "Choices")
                currentNode = GetChildNode(currentNode, path1);

                // Next steps: drill into any additional nested keys (e.g., "OptionA")
                if (extraPaths != null)
                {
                    foreach (var step in extraPaths)
                        currentNode = GetChildNode(currentNode, step);
                }

                if (currentNode is YamlScalarNode scalarValue)
                    return scalarValue.Value;
            }

            return null;
        }

    }
}