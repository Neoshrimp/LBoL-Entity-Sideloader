using HarmonyLib;
using LBoL.Base.Extensions;
using LBoLEntitySideloader.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace LBoLEntitySideloader.Reflection
{
    internal class TemplatesReflection
    {

        static HashSet<Type> templateTypes = new HashSet<Type>();


        static public HashSet<Type> AllTemplateTypes(bool refresh = false)
        { 
                if (templateTypes.Empty() || refresh)
                {
                typeof(EntityDefinition).Assembly.GetExportedTypes().Where(t => t.BaseType == typeof(EntityDefinition)).Do(t => templateTypes.Add(t));
                }
                return templateTypes;
        }


    }
}
