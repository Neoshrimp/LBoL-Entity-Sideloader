using System.IO;

namespace LBoLEntitySideloader.Resource
{
    public interface IResourceSource
    {
        public Stream Load(string id);

        public bool TryGetFileName(string id, out string name);
    }
}
