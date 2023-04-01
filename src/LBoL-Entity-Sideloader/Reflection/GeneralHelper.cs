using System;
using System.Collections.Generic;
using System.Text;

namespace LBoLEntitySideloader.Reflection
{
    public class GeneralHelper
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
    }
}
