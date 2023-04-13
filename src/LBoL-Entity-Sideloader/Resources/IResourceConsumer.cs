using System;
using System.Collections.Generic;
using System.Text;

namespace LBoLEntitySideloader.Resources
{

    public interface IResourceConsumer<T>
    {
        public void Consume(T resource);
    }
}
