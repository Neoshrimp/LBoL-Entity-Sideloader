using LBoL.Core;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using YamlDotNet.RepresentationModel;

namespace LBoLEntitySideloader.Resource
{
    public class LocalizationFiles : LocalizationOption, IResourceProvider<YamlMappingNode>
    {

        public Locale fallbackLoc = Locale.En;

        internal Dictionary<Locale, Func<YamlMappingNode>> locTable = new Dictionary<Locale, Func<YamlMappingNode>>();


        IResourceSource source;

        Func<string, YamlMappingNode> loadingAction;

        public LocalizationFiles(IResourceSource source) 
        { 
            this.source = source;
            loadingAction = (string id) => ResourceLoader.LoadYaml(id, this.source);
        }

        public LocalizationFiles(IResourceSource source, Locale fallbackLoc) : this(source)
        {
            this.fallbackLoc = fallbackLoc;
        }

        public void AddLocaleFile(Locale locale, string fileName)
        {

            fileName = Source.AddExtension(fileName, ".yaml");

            if (!locTable.TryAdd(locale, () => loadingAction(fileName)))
            {
                Log.LogDev()?.LogWarning($"{fileName}: LocalizationFiles already have {locale} registered");
            }
        }

        public YamlMappingNode Load(Locale locale)
        {
            if (locTable.TryGetValue(locale, out Func<YamlMappingNode> getYaml))
            {
                return getYaml();
            }
            Log.LogDev()?.LogWarning($"{locale} locale option does not have a file set");
            return null;
        }

        internal Dictionary<string, Dictionary<string, object>> LoadLocTable(Type entityType, Type[] entityLogicTypes)
        {

            YamlMappingNode yaml = Load(Localization.CurrentLocale);
            if (yaml == null)
            {
                Log.log.LogInfo($"Localization for {Localization.CurrentLocale} not found. Trying to use {fallbackLoc}fallback option.");
                yaml = LoadFallback();
            }


            if (yaml != null)
            {
                var termDic = Localization.InternalLoadTypeLocalizationTable(yaml, entityType, entityLogicTypes);
                return termDic;
            }
            Log.log.LogWarning($"{entityType.Assembly.GetName().Name}: No localization found for {entityType.Name} types.");
            return null;
        }


        public static void MissingValueError(string enityLogicId)
        {
            Log.log.LogWarning($"{enityLogicId} entity was registered but localization file contains no values for {enityLogicId}. Did you forget to add \"{enityLogicId}:\" entry to .yaml file?");
        }

        public YamlMappingNode LoadFallback() => Load();
            

        public YamlMappingNode Load() => Load(fallbackLoc);

        public Dictionary<string, YamlMappingNode> LoadMany()
        {
            return null;
        }
    }
}
