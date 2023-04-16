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

        private static readonly BepInEx.Logging.ManualLogSource log = BepinexPlugin.log;

        public static Type MakeGenericType(Type type, Type[] genParameters)
        {
            if (!type.IsGenericType)
            {
                log.LogWarning($"{type} is not generic");
                return null;
            }

            return type.GetGenericTypeDefinition().MakeGenericType(genParameters);
        }
 

        public static MethodInfo InnerMoveNext(Type type, string methodName)
        {
            var enumType = type.GetNestedTypes(AccessTools.allDeclared).Where(t => t.Name.Contains($"<{methodName}>")).Single();

            return AccessTools.Method(enumType, "MoveNext");
        }

        public static MethodInfo InnerMoveNext(string typeName, string methodName)
        {
            return InnerMoveNext(AccessTools.TypeByName(typeName), methodName);
        }

        public static MethodInfo InnerMoveNext(string typeName, MethodInfo methodInfo)
        {
            return InnerMoveNext(AccessTools.TypeByName(typeName), methodInfo.Name);
        }


        public static MethodInfo InnerMoveNext(Type type, MethodInfo methodInfo)
        {
            return InnerMoveNext(type, methodInfo.Name);
        }


    }
}
