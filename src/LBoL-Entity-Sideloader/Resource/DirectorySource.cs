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

        // Redundant now? Might be removed

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

        /// <summary>
        /// Finds a file of name id within the directory.
        /// Returns name for found path.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="searchSubdirectories">If true, will search for subdirectories within the path. (e.g., searching events/event.png searches for event.png in all subdirectories under "events/"). Otherwise, searches only the exact path in id.</param>
        /// <returns></returns>
        public override bool TryGetFileName(string id, out string name, bool searchSubdirectories = true)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(path))
            {
                name = null;
                return false;
            }

            // Check for exact path, slash agnostic.
            string relativePath = id.Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar);
            string fullPath = Path.Combine(path, relativePath);

            if (File.Exists(fullPath))
            {
                name = relativePath.Replace('\\', '/');
                return true;
            }

            // If recursive subfolder search is disabled, give up immediately
            if (!searchSubdirectories)
            {
                name = null;
                BepinexPlugin.log.LogWarning($"[DirectorySource] Could not find file in path: {id}");
                return false;
            }

            // Recursive search starting from the specified folder prefix (if any)
            if (dirInfo != null && dirInfo.Exists)
            {
                string targetFileName = Path.GetFileName(id); // e.g., "event.yaml" from "en/event.yaml"
                string specifiedSubDir = Path.GetDirectoryName(relativePath); // e.g., "en" from "en/event.yaml"

                // Determine search root: if "en/event.yaml" was passed, search inside "Yourmod/en/"
                string searchRootPath = string.IsNullOrEmpty(specifiedSubDir)
                    ? path
                    : Path.Combine(path, specifiedSubDir);

                if (Directory.Exists(searchRootPath))
                {
                    var searchDirInfo = new DirectoryInfo(searchRootPath);
                    foreach (var fi in searchDirInfo.EnumerateFiles("*", SearchOption.AllDirectories))
                    {
                        if (fi.Name.Equals(targetFileName, StringComparison.OrdinalIgnoreCase))
                        {
                            // Calculate relative path from the mod root directory
                            string foundRelPath = Path.GetRelativePath(path, fi.FullName).Replace('\\', '/');
                            name = foundRelPath;
                            return true;
                        }
                    }
                }
            }

            name = null;
            return false;
        }

        public override Stream Load(string id)
        {
            var filePath = FullPath(id);

            try
            {
                if (File.Exists(filePath))
                {
                    return new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                }
                Log.log.LogWarning($"{this.GetType()}: File not found at path '{filePath}' for id '{id}'");
                return null;
            }
            catch (Exception ex)
            {
                Log.log.LogError($"{this.GetType()} exception while loading file {id}: {ex}");
                return null;
            }
        }


        public string FullPath(string id)
        {
            if (string.IsNullOrEmpty(id)) return path;
            string relativePath = id.Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar);
            return Path.Combine(path, relativePath);
        }


    }

}