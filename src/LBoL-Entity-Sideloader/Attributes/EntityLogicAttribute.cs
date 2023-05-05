using System;
using System.Collections.Generic;
using System.Text;

namespace LBoLEntitySideloader.Attributes
{
    /// <summary>
    /// Used to mark entity logic classes. Ensures that there is single entity logic per definition.
    /// Currently EntityLogic type names must the same as entity Ids.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class EntityLogic : Attribute
    {

        readonly Type definitionType;
        /// <summary>
        /// definitionType should be specified like `typeof(MyCardDefinition)`
        /// </summary>
        /// <param name="definitionType">Type of entity definition (concrete template implementation) to which this entity logic class belongs to</param>
        public EntityLogic(Type definitionType)
        {
            this.definitionType = definitionType;

        }

        public Type DefinitionType => definitionType;
    }

}
