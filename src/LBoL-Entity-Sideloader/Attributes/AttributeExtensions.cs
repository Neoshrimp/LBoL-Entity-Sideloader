using BepInEx;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace LBoLEntitySideloader.Attributes
{
    public static class AttributeExtensions
    {

        public static AT SingularAttribute<AT>(this Type type, object[] allAttributes = null) where AT : Attribute
        {   
            allAttributes ??= allAttributes = type.GetCustomAttributes(inherit: false);

            var attribute = allAttributes.Where(a => a.GetType() == typeof(AT)).SingleOrDefault();

            if (attribute is AT a)
                return a;

            return null;
        }
        public static IEnumerable<AT> MultiAttribute<AT>(this Type type, object[] allAttributes = null) where AT : Attribute
        {
            allAttributes ??= allAttributes = type.GetCustomAttributes(inherit: false);

            var attributes = allAttributes.Where(a => a.GetType() == typeof(AT)).Select(a => (AT)a).AsEnumerable();

            if (attributes.Count() != 0)
                return attributes;

            return null;
        }



    }



}
