using LBoL.Core;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using LBoL.Core.Adventures;


namespace LBoLEntitySideloader
{

    public interface IConfigProvider<C> where C : class
    {
        abstract public C DefaultConfig();

        abstract public C GetConfig();
    }

    public interface IGameEntityProvider<E> where E : GameEntity
    {
    }

    // Adventure does not extend GameEntity
    public interface IAdventureProvider<A> where A : Adventure
    {
    }

    public abstract class EntityDefinition
    {
        private string id;

        private Assembly assembly;
        public string Id { get => id; set => id = value; }
        public Assembly Assembly { get => assembly; set => assembly = value; }
        

    }
}
