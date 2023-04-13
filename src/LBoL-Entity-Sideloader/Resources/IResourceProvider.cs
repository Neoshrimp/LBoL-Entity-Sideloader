using System;
using System.Collections.Generic;
using System.Text;

namespace LBoLEntitySideloader.Resources
{
    internal interface IResourceProvider<T>
    {
        public T Load();
    }
}
