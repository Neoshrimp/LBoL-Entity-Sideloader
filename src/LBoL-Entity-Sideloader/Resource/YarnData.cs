using LBoL.Core;
using LBoLEntitySideloader.Entities;
using System;
using System.Collections.Generic;
using YamlDotNet.RepresentationModel;

namespace LBoLEntitySideloader.Resource
{
    public class YarnData
    {
        public byte[] compiledBytes;
        public LocalizationFiles dialogueLocalization;
        public Locale fallbackLocale = Locale.En;

        public YarnData() { }

        public YarnData(byte[] compiledBytes, LocalizationFiles dialogueLocalization, Locale fallbackLocale = Locale.En)
        {
            this.compiledBytes = compiledBytes;
            this.dialogueLocalization = dialogueLocalization;
            this.fallbackLocale = fallbackLocale;
            if (this.dialogueLocalization != null)
            {
                this.dialogueLocalization.fallbackLoc = fallbackLocale;
            }
        }

        /// <summary>
        /// Automatically loads {AdventureId}.yarnc from yarnSource,
        /// and discovers dialogue YAML files from locSource under {AdventureId}{Locale}.yaml.
        /// Defaults fallback locale to Locale.En if active locale is missing.
        /// Sources are either BepinexPlugin.embeddedSource (if you want it to be from Resources) or .directorySource (If you want it to be from DirResources, aka exposed outside the dll)
        /// </summary>
        /// <param name="adventureTemplate"></param>
        /// <param name="yarnSource">Searches for the yarnc for the event dialogue.</param>
        /// <param name="locSource">See yarn source. Searches for the yaml for the event's dialogue.</param>
        /// <param name="fallbackLocale"></param>
        public void AutoLoad(AdventureTemplate adventureTemplate, IResourceSource yarnSource, IResourceSource locSource, Locale fallbackLocale = Locale.En)
        {
            string id = adventureTemplate.GetId();

            // 1. Load compiled Yarn binary (e.g. Eventname.yarnc)
            compiledBytes = ResourceLoader.ResourceBinary(id + ".yarnc", yarnSource);

            // 2. Discover dialogue localization YAMLs (searches top-level and all subfolders for EventnameEn.yaml)
            dialogueLocalization = new LocalizationFiles(locSource, fallbackLocale);
            dialogueLocalization.DiscoverAndLoadLocFiles(id);
        }

        public Dictionary<string, string> GetStringTableForCurrentLocale()
        {
            var stringTable = new Dictionary<string, string>();
            if (dialogueLocalization == null)
            {
                BepinexPlugin.log.LogWarning("[YarnData] dialogueLocalization is NULL!");
                return stringTable;
            }

            Locale currentLocale = Localization.CurrentLocale;
            BepinexPlugin.log.LogInfo($"[YarnData] GetStringTableForCurrentLocale called for locale: {currentLocale}");

            // Load() automatically tries currentLocale first, then falls back to fallbackLoc (Locale.En)
            YamlMappingNode yaml = dialogueLocalization.Load(currentLocale);

            if (yaml != null)
            {
                foreach (var entry in yaml.Children)
                {
                    string key = (entry.Key is YamlScalarNode kScalar) ? kScalar.Value : entry.Key.ToString();
                    string value = (entry.Value is YamlScalarNode vScalar) ? vScalar.Value : entry.Value.ToString();
                    stringTable[key] = value;
                }
                BepinexPlugin.log.LogInfo($"[YarnData] Loaded {stringTable.Count} dialogue entries from YAML.");
                foreach (var kv in stringTable)
                {
                    BepinexPlugin.log.LogInfo($"   YAML Entry: '{kv.Key}' => '{kv.Value}'");
                }
            }
            else
            {
                BepinexPlugin.log.LogWarning($"[YarnData] Loaded YAML is NULL for locale: {currentLocale} and fallback locale: {dialogueLocalization.fallbackLoc}!");
            }

            return stringTable;
        }
    }
}