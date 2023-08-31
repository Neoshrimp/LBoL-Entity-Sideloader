using HarmonyLib;
using LBoL.Base.Extensions;
using LBoLEntitySideloader.Attributes;
using LBoLEntitySideloader.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace LBoLEntitySideloader.ReflectionHelpers
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


        static public Type Template2FacType(Type template, bool refresh = false)
        {
            if (!ExpectsEntityLogic(template))
                return null;


            var interfaceType = typeof(ITypeProvider<>).GetGenericTypeDefinition();

            return template.GetInterfaces().First(i => i.IsSubclassOfGeneric(interfaceType)).GetGenericArguments().First();

        }




        static public bool IsTemplateType(Type type)
        {
            return AllTemplateTypes().Any(t => type.IsSubclassOf(t));
        }


        static public bool DoOverwrite(Type definitionType, string methodName)
        {
            var method = AccessTools.Method(definitionType, methodName);
            return method.GetCustomAttribute<DontOverwriteAttribute>(inherit: true) == null;
        }

    }
}
