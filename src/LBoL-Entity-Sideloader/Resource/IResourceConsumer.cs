using System;
using System.Collections.Generic;
using System.Text;


namespace LBoLEntitySideloader.Resource
{

    public interface IResourceConsumer<T>
    {
        public void Consume(T resource);
    }
}
