using System;
using System.Collections.Generic;
using System.Text;

namespace LBoLEntitySideloader.Resource
{
    /// <summary>
    /// Should have been called batch localization.
    /// </summary>
    public class GlobalLocalization : LocalizationOption
    {
        public readonly LocalizationFiles LocalizationFiles;

        public GlobalLocalization(IResourceSource source)
        {
            LocalizationFiles = new LocalizationFiles(source, mergeTerms: false);
        }

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
