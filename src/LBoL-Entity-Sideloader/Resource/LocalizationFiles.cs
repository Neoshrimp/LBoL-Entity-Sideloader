using HarmonyLib;
using LBoL.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using YamlDotNet.Helpers;
using YamlDotNet.RepresentationModel;

namespace LBoLEntitySideloader.Resource
{
    public class LocalizationFiles : LocalizationOption, IResourceProvider<YamlMappingNode>
    {

        public Locale fallbackLoc = Locale.En;

        internal Dictionary<Locale, Func<YamlMappingNode>> locTable = new Dictionary<Locale, Func<YamlMappingNode>>();

        public Dictionary<Locale, string> fileNames = new Dictionary<Locale, string>();

        /// <summary>
        /// should localization terms (Name, Description etc.) should be merged
        /// </summary>
        public bool mergeTerms = false;

        public readonly IResourceSource source;

        Func<string, YamlMappingNode> loadingAction;

        public LocalizationFiles(IResourceSource source) 
        { 
            this.source = source;
            loadingAction = (string id) => ResourceLoader.LoadYaml(id, this.source);
        }

        public LocalizationFiles(IResourceSource source, bool mergeTerms) : this(source)
        { 
            this.mergeTerms = mergeTerms;
        }

        public LocalizationFiles(IResourceSource source, Locale fallbackLoc) : this(source)
        {
            this.fallbackLoc = fallbackLoc;
        }


        public LocalizationFiles(IResourceSource source, Locale fallbackLoc, bool mergeTerms) : this(source, fallbackLoc)
        { 
            this.mergeTerms = mergeTerms;
        }


        public void AddLocaleFile(Locale locale, string fileName)
        {

            fileName = Source.AddExtension(fileName, ".yaml");


            if (!locTable.TryAdd(locale, () => loadingAction(fileName)))
            {
                Log.LogDev()?.LogWarning($"{fileName}: LocalizationFiles already have {locale} registered");
            }
            else
            {
                fileNames.TryAdd(locale, fileName);
            }
        }

        public Locale GetAvailableLocale()
        {
            return locTable.ContainsKey(Localization.CurrentLocale) ? Localization.CurrentLocale : fallbackLoc;
        }


        public YamlMappingNode Load(Locale locale)
        {
            if (locTable.TryGetValue(locale, out Func<YamlMappingNode> getYaml))
            {
                return getYaml();
            }
            else if (locTable.TryGetValue(fallbackLoc, out getYaml))
            {
                var fileName = fileNames.GetValueSafe(fallbackLoc);
                Log.LogDev()?.LogInfo($"Localization for {Localization.CurrentLocale} not found. Trying to use {fallbackLoc} fallback option from file: {fileName}.");
                return getYaml();
            }
            Log.LogDev()?.LogWarning($"{this.GetType().Name}: {locale} locale option does not have a file set.");
            return null;
        }

        internal Dictionary<string, Dictionary<string, object>> LoadLocTable(Type entityType, Type[] entityLogicTypes)
        {

            YamlMappingNode yaml = Load(Localization.CurrentLocale);


            if (yaml != null)
            {
                var termDic = Localization.InternalLoadTypeLocalizationTable(yaml, entityType, entityLogicTypes);
                return termDic;
            }
            Log.log.LogWarning($"{entityType.Assembly.GetName().Name}: No localization found for {entityType.Name} types.");
            return null;
        }


        internal Dictionary<string, Dictionary<string, object>> LoadLocTable(IEnumerable<string> Ids, bool addEmptyDic = true)
        {
            Dictionary<string, Dictionary<string, object>> dictionary = new Dictionary<string, Dictionary<string, object>>();

            YamlMappingNode yaml = Load(Localization.CurrentLocale);

            if (yaml == null)
            {
                Log.log.LogWarning($"{nameof(LocalizationFiles)}: No localization file found.");
                return dictionary;
            }

            IOrderedDictionary<YamlNode, YamlNode> children = yaml.Children;
            foreach (var id in Ids)
            {
                if (children.TryGetValue(id, out var yamlNode))
                {
                    YamlMappingNode yamlMappingNode = yamlNode as YamlMappingNode;
                    if (yamlMappingNode != null)
                    {
                        dictionary.Add(id, Localization.CreatePropertyLocalizeTable(id, yamlMappingNode));
                        continue;
                    }
                }
                if(addEmptyDic)
                    dictionary.Add(id, new Dictionary<string, object>());
            }


            return dictionary;
        }


        internal Dictionary<string, Dictionary<string, object>> LoadLocTable(IEnumerable<IdContainer> idContainers, bool addEmptyDic = true)
        {
            return LoadLocTable(idContainers.Select(id => id.ToString()), addEmptyDic);
        }



        public static void MissingValueError(string enityLogicId)
        {
            Log.log.LogWarning($"{enityLogicId} entity was registered but localization file contains no values for {enityLogicId}. Did you forget to add \"{enityLogicId}:\" entry to .yaml file?");
        }

                   

        public YamlMappingNode Load() => Load(fallbackLoc);

        public Dictionary<string, YamlMappingNode> LoadMany()
        {
            return null;
        }


    }
}
