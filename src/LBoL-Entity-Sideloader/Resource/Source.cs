using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace LBoLEntitySideloader.Resource
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

        public static string AddExtension(string name, string extension)
        {
            if (!name.EndsWith(extension))
            {
                return name + extension;
            }
            return name;
        }
    }
}
