using LBoL.Core;
using LBoLEntitySideloader.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using YamlDotNet.RepresentationModel;

namespace LBoLEntitySideloader.Resource
{
    public class YarnData
    {
        public byte[] compiledBytes;
        public LocalizationFiles dialogueLocalization;

        public YarnData() { }

        public YarnData(byte[] compiledBytes, LocalizationFiles dialogueLocalization)
        {
            this.compiledBytes = compiledBytes;
            this.dialogueLocalization = dialogueLocalization;
        }

        /// <summary>
        /// Automatically loads {AdventureId}.yarnc from yarnSource under {AdventureId}.yarnc,
        /// and discovers event dialogue YAML files from locSource under {AdventureId}{Locale}.yaml.
        /// </summary>
        public void AutoLoad(AdventureTemplate adventureTemplate, IResourceSource yarnSource, IResourceSource locSource)
        {
            string id = adventureTemplate.GetId(); // "YuukaGarden"

            // 1. Load compiled Yarn binary (e.g. YuukaGarden.yarnc)
            compiledBytes = ResourceLoader.ResourceBinary(id + ".yarnc", yarnSource);

            // 2. Discover dialogue localization YAMLs (searches top-level and all subfolders for YuukaGardenEn.yaml)
            dialogueLocalization = new LocalizationFiles(locSource);
            dialogueLocalization.DiscoverAndLoadLocFiles(id);
        }

        /// <summary>
        /// Loads dialogue line strings for the current locale as a dictionary of lineId -> text.
        /// </summary>
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
                BepinexPlugin.log.LogWarning($"[YarnData] Loaded YAML is NULL for locale: {currentLocale}!");
            }
            return stringTable;
        }
    }
}