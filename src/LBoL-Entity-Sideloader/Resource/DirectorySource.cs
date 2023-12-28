using BepInEx;  
using LBoLEntitySideloader.Resource;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace LBoLEntitySideloader.Resource
{
    public class DirectorySource : Source
    {

        string path;

        public DirectoryInfo dirInfo;

        HashSet<string> fileNames= new HashSet<string>(new IdEqualFilename());

        class IdEqualFilename : IEqualityComparer<string>
        {
            public bool Equals(string x, string y)
            {
                return y.StartsWith(x);
            }

            public int GetHashCode(string obj)
            {
                return obj.GetHashCode();
            }
        }

        public DirectorySource(string path)
        {
            this.path = path;
            this.dirInfo = new DirectoryInfo(path);
        }

        /// <summary>
        /// plugin dir + path.
        /// </summary>
        /// <param name="GUID"></param>
        /// <param name="subFolder"></param>
        public DirectorySource(string GUID, string subFolder)
        {
            try
            {
                var pluginInfo = BepInEx.Bootstrap.Chainloader.PluginInfos[GUID];

                var pluginLoc = pluginInfo.Location;

                if (pluginLoc == null)
                {
                    // assume mod is loaded through script loader
                    // should never happen if scriptengine is up to date with my fork
                    this.path = Path.Combine(Path.GetDirectoryName(Paths.PluginPath), subFolder);
                }
                else
                {
                    this.path = Path.Combine(Path.GetDirectoryName(pluginLoc), subFolder);
                }

                this.dirInfo = new DirectoryInfo(path);

            }
            catch (Exception ex)
            {
                path = "";
                Log.log.LogError(ex);
            }
        }

        public override bool TryGetFileName(string id, out string name)
        {
            id = LegalizeFileName(id);
            // breaks if file is deleted
            if (fileNames.Contains(id))
            {
                name = id;
                return true;
            }

            foreach(var fi in dirInfo.EnumerateFiles())
            {
                fileNames.Add(fi.Name);
                if (fi.Name.StartsWith(id))
                {
                    name = id;
                    return true;
                }
            }
            name = null;
            return false;
        }


        public string FullPath(string id)
        {
            id = LegalizeFileName(id);

            var filePath = Path.Combine(path, id);

            return filePath;
        }

        public override Stream Load(string id)
        {
            var filePath = FullPath(id);

            try
            {
                FileStream stream = new FileStream(filePath, FileMode.Open);
                return stream;
            }
            catch (IOException ex)
            {
                Log.log.LogError($"{this.GetType()} exception while loading file {id}: {ex}");
                return null;
            }


        }


    }

}