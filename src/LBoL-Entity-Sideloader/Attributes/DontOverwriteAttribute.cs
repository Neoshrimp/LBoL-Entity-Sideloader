using System;
using System.Collections.Generic;
using System.Text;

namespace LBoLEntitySideloader.Attributes
{
    [System.AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    sealed class DontOverwriteAttribute : Attribute
    {

        // This is a positional argument
        public DontOverwriteAttribute()
        {
        }


    }
}
