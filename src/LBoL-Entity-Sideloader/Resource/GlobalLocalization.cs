using LBoL.Core;
using LBoLEntitySideloader.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LBoLEntitySideloader.Resource
{
    /// <summary>
    /// Should have been called BatchLocalization.
    /// </summary>
    [Obsolete("Use BatchLocalization instead", error: false)]
    public class GlobalLocalization : LocalizationOption
    {
        public readonly LocalizationFiles LocalizationFiles;

        public GlobalLocalization(IResourceSource source)
        {
            LocalizationFiles = new LocalizationFiles(source, mergeTerms: false);
        }

        /// <summary>
        /// mergeTerms is misleading here. 
        /// It will only apply if localization files are set for the first time for the type
        /// </summary>
        /// <param name="source"></param>
        /// <param name="mergeTerms"></param>
        public GlobalLocalization(IResourceSource source, bool mergeTerms) : this(source)
        {
            LocalizationFiles.mergeTerms = mergeTerms;
        }

        public GlobalLocalization(LocalizationFiles localizationFiles)
        {
            LocalizationFiles = localizationFiles;
        }

        /// <summary>
        /// Automatically discovers localization files in given resource source.
        /// Files names should follow convention of {templateType} + {2 letter lang code} + ".yaml", i.e., UltimateSkillKo.yaml.
        /// Does NOT work with certain templates which do not implement EntityType.
        /// </summary>
        /// <param name="entityTemplate">concrete template instance, probably `this`</param>
        [Obsolete("Use the other overload")]
        public void DiscoverAndLoadLocFiles(EntityDefinition entityTemplate)
        {
            var templateName = entityTemplate.EntityType().Name;
            DiscoverAndLoadLocFiles(templateName);
        }

        /// <summary>
        /// Automatically discovers localization files in given resource source.
        /// Files names should follow convention of {templateType} + {2 letter lang code} + ".yaml", i.e.,  UltimateSkillKo.yaml
        /// </summary>
        /// <param name="templateName">general template type</param>
        public void DiscoverAndLoadLocFiles(string templateName)
        {
            foreach (var l in Enum.GetValues(typeof(Locale)).Cast<Locale>())
            {
                var id = $"{templateName}{l}";
                if (LocalizationFiles.source.TryGetFileName(Source.AddExtension(id, ".yaml"), out var _))
                    LocalizationFiles.AddLocaleFile(l, id);
            }
        }
    }
}
