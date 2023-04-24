using HarmonyLib;
using LBoL.Base.Extensions;
using LBoLEntitySideloader.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace LBoLEntitySideloader.Reflection
{
    internal class TemplatesReflection
    {

        static HashSet<Type> templateTypes = new HashSet<Type>();

        static List<Type> templatesExpectingEntityLogic = new List<Type>();



        static public HashSet<Type> AllTemplateTypes(bool refresh = false)
        { 
            if (templateTypes.Empty() || refresh)
            {
                typeof(EntityDefinition).Assembly.GetExportedTypes().Where(t => t.BaseType == typeof(EntityDefinition)).Do(t => templateTypes.Add(t));
            }
            return templateTypes;
        }


        static public bool ExpectsEntityLogic(Type template, bool refresh = false)
        {

            if (templatesExpectingEntityLogic.Empty() || refresh)
            {

                var interfaceType = typeof(ITypeProvider<>).GetGenericTypeDefinition();
                AllTemplateTypes().Where(t => t.GetInterfaces().Any(i => i.IsSubclassOfGeneric(interfaceType))).Do(t => templatesExpectingEntityLogic.Add(t));
            }


            return templatesExpectingEntityLogic.Any(t => template.IsSubclassOf(t) || t == template);
        }

    }
}
