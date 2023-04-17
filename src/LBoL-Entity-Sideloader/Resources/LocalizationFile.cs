using LBoL.Core;
using System;
using System.Collections.Generic;
using System.Text;
using YamlDotNet.RepresentationModel;

namespace LBoLEntitySideloader.Resources
{
    public class LocalizationFile
    {

        Locale locale;
        Locale fallbackLoc = Locale.En;

        Action<YamlMappingNode> load;

        public LocalizationFile(Action<YamlMappingNode> load)
        {
            this.load = load;
        }
    }
}
