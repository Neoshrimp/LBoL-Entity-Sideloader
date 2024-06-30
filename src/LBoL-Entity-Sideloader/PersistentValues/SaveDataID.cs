using System;
using System.Linq;

namespace LBoLEntitySideloader.PersistentValues
{
    public struct SaveDataID
    {
        public string GUID;
        public string Name;
        public string midfix;

        public const string suffix = ".modd";

        public override string ToString() => $"{GUID}_{midfix}_{Name}";
        public string GetFileName(int profileIndex) => $"{this}_{profileIndex}{suffix}";



/*        public int GetIndex(string fileName) 
        {
            return Int32.Parse(fileName.Split("_").Last().Split(".").First());
        }*/
    }
}
