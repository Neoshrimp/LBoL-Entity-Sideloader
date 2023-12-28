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
using static LBoLEntitySideloader.BepinexPlugin;


namespace LBoLEntitySideloader.Resource
{
    public class EmbeddedSource : Source
    {
        Assembly assembly;

        //ResourceManager resourceManager;

        public EmbeddedSource(Assembly assembly)
        {
            this.assembly = assembly;

        }


        public override bool TryGetFileName(string id, out string name)
        {
            id = LegalizeFileName(id);
            name = assembly.GetManifestResourceNames().FirstOrDefault(n => n.EndsWith(id));
            return name != null;
        }


        public override Stream Load(string id)
        {

            TryGetFileName(id, out var fullName);

            if (fullName == null) 
            {
                log.LogWarning($"{assembly.GetName().Name}: no embedded file found with name {id}");
                return null;
            }

            return assembly.GetManifestResourceStream(fullName);
        }


    }
}
