using System.IO;

namespace LBoLEntitySideloader.Resource
{
    public interface IResourceSource
    {
        public Stream Load(string id);

        /// <summary>
        /// Checks if a file exists.
        /// </summary>
        /// <param name="id">Relative path or file name</param>
        /// <param name="name">The resolved relative path if found</param>
        /// <param name="searchSubdirectories">If true, searches subdirectories when direct match fails</param>
        public bool TryGetFileName(string id, out string name, bool searchSubdirectories = true);
    }
}
