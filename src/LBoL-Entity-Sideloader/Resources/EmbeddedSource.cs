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

            //c#, regex to check if file name ends with common picture file extension 

            id = LegalizeFileName(id);



            var fullName = assembly.GetManifestResourceNames().FirstOrDefault(n => n.EndsWith(id));

            if (fullName == null) 
            {
                log.LogWarning($"{assembly.GetName().Name}: no embedded file found with name {id}");
                return null;
            }

            return assembly.GetManifestResourceStream(fullName);
        }
    }
}
