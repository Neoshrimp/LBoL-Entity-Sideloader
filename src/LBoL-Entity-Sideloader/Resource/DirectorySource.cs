using BepInEx;  
using LBoLEntitySideloader.Resource;
using System;
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

        public DirectorySource(string path)
        {
            this.path = path;
            this.dirInfo = new DirectoryInfo(path);
        }

        /// <summary>
        /// plugin dir + path.
        /// If mod is loaded with a script engine this defaults to BeInEx/plugins + subFolder
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
                    this.path = Path.Combine(Path.GetDirectoryName(Paths.PluginPath), subFolder);
                }
                else
                {
                    this.path = Path.Combine(Path.GetDirectoryName(pluginLoc), subFolder);
                }

                this.dirInfo = new DirectoryInfo(path);

            }
            catch (Exception e)
            {
                path = "";
                Log.log.LogError(e);
            }
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
                Log.log.LogError(ex);
                return null;
            }


        }
    }

}