using HarmonyLib;
using LBoL.Base.Extensions;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Adventures;
using LBoL.Core.GapOptions;
using LBoL.Core.StatusEffects;
using LBoL.Core.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LBoLEntitySideloader.Reflection
{
    public class ReflectionHelpers
    {

        public class Config
        {
            private static readonly BepInEx.Logging.ManualLogSource log = BepinexPlugin.log;

            static string[] potentialFromIdNames = new string[] { "FromId", "FromName", "FromLevel", "FromID" };

            static readonly List<Assembly> configAssemblies = new List<Assembly>() { typeof(CardConfig).Assembly };

            static Dictionary<Type, MethodInfo> fromIdCache = new Dictionary<Type, MethodInfo>();

            static HashSet<Type> configTypeCache = new HashSet<Type>();

            public static MethodInfo GetFromIdMethod(Type configType, bool verify = true)
            {

                if (fromIdCache.TryGetValue(configType, out MethodInfo result))
                    return result;

                MethodInfo mFromId = null;

                foreach (var n in potentialFromIdNames)
                {
                    mFromId = AccessTools.Method(configType, n);
                    if (mFromId != null)
                    {
                        break;
                    }
                }

                if (mFromId == null)
                {
                    throw new MissingMemberException($"None of the potential fromId names managed to reflect a method from {configType}");
                }

                if (verify)
                {
                    var parameters = mFromId.GetParameters();
                    if (!(parameters.Count() == 1))
                    {
                        log.LogError($"{configType}: {mFromId.Name} did not have expected number of parameters. {parameters.Count()} instead of 1.");
                    }
                    if (!parameters.All(pi => pi.ParameterType == typeof(string)))
                    {
                        log.LogError($"{configType}: {mFromId.Name} not all parameters are string type");
                    }
                }

                fromIdCache.Add(configType, mFromId);

                return mFromId;
            }

            public static IEnumerable<Type> GetAllConfigTypes(IEnumerable<Assembly> assemblies = null, bool refresh = false)
            {
                if (assemblies == null)
                    assemblies = configAssemblies;

                if (refresh && configTypeCache.Empty())
                {
                    foreach (var a in assemblies)
                    {
                        a.GetExportedTypes().Where(t => t.IsSealed && t.Name.EndsWith("Config")).Do(t => configTypeCache.Add(t));
                    }
                }

                return configTypeCache;
            }

            internal void ClearCache()
            {
                fromIdCache = new Dictionary<Type, MethodInfo>();
                configTypeCache = new HashSet<Type>();
            }



            public static void LogFromIdNames()
            {
                var types = GetAllConfigTypes();

                types.Do(t => log.LogInfo(t.Name));

                log.LogInfo($"----{types.Count()}");


                types.SelectMany(t => AccessTools.GetDeclaredMethods(t)).Where(m => !m.IsSpecialName && !m.Name.StartsWith("<")).
                    Do(m => log.LogInfo($"{m.DeclaringType}: {m.Name}"));

                var methods = types.Select(t => AccessTools.GetDeclaredMethods(t).Where(m => !m.IsSpecialName && !m.Name.StartsWith("<")).ToHashSet(new CompareByMethodName()));

                var u = methods.Aggregate((s1, s2) => s1.Union(s2).ToHashSet(new CompareByMethodName()));
                var v = methods.Aggregate((s1, s2) => s1.Intersect(s2).ToHashSet(new CompareByMethodName()));

                log.LogInfo("----");

                u.Except(v).Do(m => log.LogInfo($"{m.DeclaringType}: {m.Name}"));

                // test
                methods.Aggregate((s1, s2) => s1.Union(s2).Except(s1.Intersect(s2)).ToHashSet()).
                Do(m => log.LogInfo($"{m.DeclaringType}: {m.Name}"));

            }

        }

        class CompareByMethodName : IEqualityComparer<MethodInfo>
        {
            public bool Equals(MethodInfo x, MethodInfo y)
            {
                return x.Name == y.Name;
            }

            public int GetHashCode(MethodInfo obj)
            {
                return obj.Name.GetHashCode();
            }
        }

        public class General
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

        public class TypeFactory
        {
            // from Library.RegisterAllAsync. No sensible common pattern exists to extract these types reflectively
            public static HashSet<Type> factoryTypes = new HashSet<Type>() { typeof(Stage), typeof(CardConfig), typeof(UltimateSkill), typeof(Exhibit), typeof(PlayerUnit), typeof(EnemyUnit), typeof(StatusEffect), typeof(Doll), typeof(GapOption), typeof(Intention), typeof(Adventure), typeof(JadeBox) };
        }
    }

}
