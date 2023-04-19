using System;
using System.Collections.Generic;
using System.Text;

namespace LBoLEntitySideloader.Resources
{
    public class GlobalLocalization : LocalizationOption
    {
        public readonly LocalizationFiles LocalizationFiles;


        public GlobalLocalization()
        {
        }

        public GlobalLocalization(LocalizationFiles localizationFiles)
        {
            LocalizationFiles = localizationFiles;
        }
    }
}
