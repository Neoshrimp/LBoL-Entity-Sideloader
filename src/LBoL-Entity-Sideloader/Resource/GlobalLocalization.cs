using System;
using System.Collections.Generic;
using System.Text;

namespace LBoLEntitySideloader.Resource
{
    /// <summary>
    /// Should have been called BatchLocalization.
    /// </summary>
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
    }
}
