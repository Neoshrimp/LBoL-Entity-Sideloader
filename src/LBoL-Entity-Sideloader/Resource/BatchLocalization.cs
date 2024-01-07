using LBoL.Core;
using LBoLEntitySideloader.Entities;
using LBoLEntitySideloader.ReflectionHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace LBoLEntitySideloader.Resource
{
    public class BatchLocalization : LocalizationOption
    {
        public readonly IResourceSource resourceSource;

        public readonly LocalizationFiles localizationFiles;

        public readonly HashSet<IdContainer> entityIds = new HashSet<IdContainer>();

        public readonly Type templateType;

        public readonly Type factoryType;


        //internal bool IsProcessed { get; set; } = false;

        public BatchLocalization(IResourceSource resourceSource, Type templateType, bool mergeTerms = false)
        {
            this.resourceSource = resourceSource;
            this.localizationFiles = new LocalizationFiles(resourceSource, mergeTerms);
            this.templateType = templateType;
            this.factoryType = TemplatesReflection.Template2FacType(templateType);
        }

        internal void RegisterSelf(Assembly userAss)
        {
            if (!TemplatesReflection.AllTemplateTypes().Contains(templateType))
                throw new ArgumentException($"{this.GetType().Name}: {templateType} is not a general template type (CardTemplate, StatusEffectTemplate etc.)");

            var tracker = UniqueTracker.Instance.batchLocalization;
            tracker.TryAdd(userAss, new Dictionary<Type, HashSet<BatchLocalization>>());
            tracker[userAss].TryAdd(templateType, new HashSet<BatchLocalization>());
            tracker[userAss][templateType].Add(this);
        }

        /// <summary>
        /// Adds default locale file.
        /// </summary>
        /// <param name="resourceSource"></param>
        /// <param name="templateType"></param>
        /// <param name="defaultLocale"></param>
        /// <param name="fileName"></param>
        public BatchLocalization(IResourceSource resourceSource, Type templateType, Locale defaultLocale, string fileName, bool mergeTerms = false) : this(resourceSource, templateType, mergeTerms)
        {
            localizationFiles.AddLocaleFile(defaultLocale, fileName);
            localizationFiles.fallbackLoc = defaultLocale;
        }


        /// <summary>
        /// Register an entity for that particular batch localization.
        /// The corresponding batch yaml file(s) 
        /// </summary>
        /// <param name="entityTemplate"></param>
        /// <returns>The same BatchLocalization which should be returned by LoadLocalization</returns>
        public BatchLocalization AddEntity(EntityDefinition entityTemplate)
        {
            if (entityTemplate.TemplateType() != templateType)
                throw new ArgumentException($"{entityTemplate.GetId()} of type {entityTemplate.TemplateType().Name} does not match target template type of {this.GetType().Name}, {templateType.Name}.");

            entityIds.Add(entityTemplate.UniqueId);
            return this;
        }

        /// <summary>
        /// Automatically tries to discover localization files for any possible locale. 
        /// Calls DiscoverAndLoadLocFiles(fileNamePrefix).
        /// </summary>
        /// <param name="resourceSource"></param>
        /// <param name="templateType"></param>
        /// <param name="fileNamePrefix"></param>
        /// <param name="fallbackLocale"></param>
        /// <param name="mergeTerms"></param>
        public BatchLocalization(IResourceSource resourceSource, Type templateType, string fileNamePrefix, Locale fallbackLocale = Locale.En, bool mergeTerms = false) : this(resourceSource, templateType, mergeTerms)
        {
            DiscoverAndLoadLocFiles(fileNamePrefix);
            localizationFiles.fallbackLoc = fallbackLocale;
        }



        /// <summary>
        /// Automatically discovers localization files in given resource source. 
        /// Should be called once per instance of BatchLocalization. 
        /// </summary>
        /// <param name="fileNamePrefix">File name without locale code and ".yaml", i.e, Cards</param>
        public void DiscoverAndLoadLocFiles(string fileNamePrefix)
        {
            foreach (var l in Enum.GetValues(typeof(Locale)).Cast<Locale>())
            {
                var id = $"{fileNamePrefix}{l}";
                if (localizationFiles.source.TryGetFileName(Source.AddExtension(id, ".yaml"), out var _))
                    localizationFiles.AddLocaleFile(l, id);
            }
        }

    }
}
