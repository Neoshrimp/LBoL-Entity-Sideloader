using System;
using System.Collections.Generic;
using System.Text;

namespace LBoLEntitySideloader.Entities.DynamicTemplates
{
    public interface IDynamicTemplate<T> where T : EntityDefinition
    {
        public T Create(IdContainer Id, UserInfo user);

        public static void SetUser(T template, UserInfo user)
        {
            template.user = user;
            template.userAssembly = user.assembly;
        }

    }
}
