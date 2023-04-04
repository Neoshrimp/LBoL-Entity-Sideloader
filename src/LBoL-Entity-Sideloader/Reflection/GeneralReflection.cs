using System;

namespace LBoLEntitySideloader.ReflectionHelpers
{
    public class GeneralReflection
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
