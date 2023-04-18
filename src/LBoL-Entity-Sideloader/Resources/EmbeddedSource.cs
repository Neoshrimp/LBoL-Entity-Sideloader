using Mono.Cecil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;

namespace LBoLEntitySideloader.Resources
{
    public class EmbeddedSource : Source
    {
        Assembly assembly;

        //ResourceManager resourceManager;

        public EmbeddedSource(Assembly assembly)
        {
            this.assembly = assembly;

        }

        public override Stream Load(string id)
        {



            var fullName = assembly.GetManifestResourceNames().First(n => n.EndsWith(id));

            return assembly.GetManifestResourceStream(fullName);
        }
    }
}
