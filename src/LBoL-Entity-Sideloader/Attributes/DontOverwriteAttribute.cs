using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace LBoLEntitySideloader.Attributes
{
    /// <summary>
    /// Used on template class overridden methods.
    /// Will have no effect unless template uses [OverwriteVanilla] attribute.
    /// The annotated method will be ignored completely and never called, 
    /// effectively preventing the change of the respective component.
    /// This attribute cannot be used on types but for entities which have EntityLogic type component entity logic will not be changed if a type with [EntityLogic] is omitted.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public sealed class DontOverwriteAttribute : Attribute
    {
        public DontOverwriteAttribute()
        {

        }

    }
}
