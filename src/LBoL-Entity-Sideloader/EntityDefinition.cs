using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace LBoLEntitySideloader
{

    public interface IConfigProvider
    {
    }

    public abstract class EntityDefinition<T, C> : IConfigProvider where T : class where C : class
    {
        private string id;

        private Assembly assembly;
        public string Id { get => id; set => id = value; }
        public Assembly Assembly { get => assembly; set => assembly = value; }
        
        
        //public abstract EntityDefinition<T,C> Init();


        public abstract C GetConfig();



    }
}
