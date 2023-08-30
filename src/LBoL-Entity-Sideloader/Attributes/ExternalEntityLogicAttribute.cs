using System;
using System.Collections.Generic;
using System.Text;

namespace LBoLEntitySideloader.Attributes
{
    /// <summary>
    /// Suppress empty
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class ExternalEntityLogicAttribute : Attribute
    {

        public ExternalEntityLogicAttribute()
        {
        }

    }
}
