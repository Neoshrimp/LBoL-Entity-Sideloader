using System.IO;

namespace LBoLEntitySideloader.Resources
{
    public interface IResourceSource
    {
        public Stream Load(string id);
    }
}
