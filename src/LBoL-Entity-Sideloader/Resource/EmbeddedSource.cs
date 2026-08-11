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


        public override bool TryGetFileName(string id, out string name, bool searchSubdirectories = true)
        {
            if (string.IsNullOrEmpty(id) || assembly == null)
            {
                name = null;
                return false;
            }

            // Convert slashes to dots (e.g. "events/YuukaGardenEn.yaml" -> "events.YuukaGardenEn.yaml")
            string dotPath = LegalizeFileName(id).Replace('/', '.').Replace('\\', '.');
            string fileNameOnly = Path.GetFileName(id);

            string[] resourceNames = assembly.GetManifestResourceNames();

            name = resourceNames.FirstOrDefault(n =>
            // Case 1: it ends with the specific file path. E.G. 'en/thing.yaml' -> First thing that has that exact path within 'yourmod'
                n.EndsWith("." + dotPath, StringComparison.OrdinalIgnoreCase) ||
            // Case 2: It exactly equals the path. 'en/thing.yaml' -> 'yourmod/en/thing.yaml'
                n.Equals(dotPath, StringComparison.OrdinalIgnoreCase) ||
            // Case 3: It finds any file with the same file name. 'en/thing.yaml' -> any file 'thing.yaml' within 'yourmod/en'
                (searchSubdirectories && n.EndsWith("." + fileNameOnly, StringComparison.OrdinalIgnoreCase))
            );

            if (name == null)
            {
                Log.log.LogWarning($"[EmbeddedSource] File '{id}' not found in embedded resources for '{assembly.GetName().Name}'.");
                return false;
            }

            return true;
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
