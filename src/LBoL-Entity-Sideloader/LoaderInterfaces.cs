using System;
using System.Collections.Generic;
using System.Text;

namespace LBoLEntitySideloader
{
    public interface IEntityLoader<T, C> where T : class where C : class
    {
        EntityDefinition<T, C> LoadEntity();
    }


    public interface IAssetLoader<T>
    {
        
    }


    public interface ResourceSourceType
    {

    }

}
