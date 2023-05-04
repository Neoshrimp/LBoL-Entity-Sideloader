using System;
using System.Collections.Generic;
using System.Text;

namespace LBoLEntitySideloader.Resource
{
    public class GlobalLocalization : LocalizationOption
    {
        public readonly LocalizationFiles LocalizationFiles;

        public GlobalLocalization(IResourceSource source)
        {
            LocalizationFiles = new LocalizationFiles(source);
        }

        public GlobalLocalization(LocalizationFiles localizationFiles)
        {
            LocalizationFiles = localizationFiles;
        }
    }
}
