using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace LBoLEntitySideloader.Resources
{
    abstract public class Source : IResourceSource
    {
        public abstract Stream Load(string id);

        public static string LegalizeFileName(string fileName)
        {
            string illegalCharsPattern = @"[\|\>\<:""\\/\\\?\*]";
            Regex regex = new Regex(illegalCharsPattern);
            string validFileName = regex.Replace(fileName, "");
            return validFileName;
        }
    }
}
