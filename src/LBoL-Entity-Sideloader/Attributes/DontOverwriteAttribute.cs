using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace LBoLEntitySideloader.Attributes
{
    [System.AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public sealed class DontOverwriteAttribute : Attribute
    {

        public DontOverwriteAttribute()
        {

        }



    }
}
