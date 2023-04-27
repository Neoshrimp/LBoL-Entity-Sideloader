using LBoLEntitySideloader.Resource;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LBoLEntitySideloader.Resource
{
    public class DirectorySource : Source
    {

        string path;

        public DirectorySource(string path)
        {
            this.path = path;
        }

        public override Stream Load(string id)
        {


            var filePath = Path.Combine(path, id);
            FileStream stream = new FileStream(filePath, FileMode.Open);


            return stream;
        }
    }

}
