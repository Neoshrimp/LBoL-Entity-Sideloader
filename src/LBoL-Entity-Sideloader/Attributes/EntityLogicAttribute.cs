using System;
using System.Collections.Generic;
using System.Text;

namespace LBoLEntitySideloader.Attributes
{

    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class EntityLogic : Attribute
    {

        readonly Type definitionType;

        public EntityLogic(Type definitionType)
        {
            this.definitionType = definitionType;

        }

        public Type DefinitionType => definitionType;
    }

}
