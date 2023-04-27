using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

namespace LBoLEntitySideloader.Resource
{
    internal interface IResourceProvider<T>
    {

        public T Load();

        public Dictionary<string, T> LoadMany();

    }
}
