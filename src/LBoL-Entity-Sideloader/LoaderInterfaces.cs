using System;
using System.Collections.Generic;
using System.Text;

namespace LBoLEntitySideloader
{
    public interface IConfigSource<C> where C : class
    {
        C LoadConfig(C deez);
    }



    public interface IFileSource
    {

    }

}
