using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace LBoLEntitySideloader.ReflectionHelpers
{
    public static class ExtraAccess
    {

        public static MethodInfo CoroutineLogic(Type type, string methodName)
        {
            var enumType = type.GetNestedTypes(AccessTools.allDeclared).Where(t => t.Name.Contains($"<{methodName}>")).Single();

            return AccessTools.Method(enumType, "MoveNext");
        }

        public static MethodInfo CoroutineLogic(string typeName, string methodName)
        {
            return CoroutineLogic(AccessTools.TypeByName(typeName), methodName);
        }

        public static MethodInfo CoroutineLogic(string typeName, MethodInfo methodInfo)
        {
            return CoroutineLogic(AccessTools.TypeByName(typeName), methodInfo.Name);
        }


        public static MethodInfo CoroutineLogic(Type type, MethodInfo methodInfo)
        {
            return CoroutineLogic(type, methodInfo.Name);
        }


    }
}
