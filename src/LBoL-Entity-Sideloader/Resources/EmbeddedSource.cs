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
    public class EmbeddedSource : IResourceSource
    {
        Assembly assembly;

        //ResourceManager resourceManager;

        public EmbeddedSource(Assembly assembly)
        {
            this.assembly = assembly;
            //resourceManager = new ResourceManager(assembly.GetName().Name+ ".Properties.Resources", assembly);
        }

        public Stream Load(string id)
        {
            /*            var res = resourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true);

                        foreach (var r in res)
                        {
                            UnityEngine.Debug.Log(((DictionaryEntry)r).Key);
                        }*/

            //return resourceManager.GetStream(id);


            var fullName = assembly.GetManifestResourceNames().First(n => n.Contains(id));

            return assembly.GetManifestResourceStream(fullName);
        }
    }
}
