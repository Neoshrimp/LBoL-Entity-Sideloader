using HarmonyLib;
using LBoL.Base.Extensions;
using LBoL.ConfigData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LBoLEntitySideloader.ReflectionHelpers
{

    public class ConfigReflection
    {
        private static readonly BepInEx.Logging.ManualLogSource log = BepinexPlugin.log;


        static readonly List<Assembly> configAssemblies = new List<Assembly>() { typeof(CardConfig).Assembly };

        

        static HashSet<Type> configTypeCache = new HashSet<Type>();


        static Dictionary<Type, FieldInfo> indexFields = new Dictionary<Type, FieldInfo>()
        {
            {
                typeof(CardConfig),
                AccessTools.Field(typeof(CardConfig), nameof(CardConfig.Index))
            },

            {
                typeof(AdventureConfig),
                AccessTools.Field(typeof(AdventureConfig), nameof(AdventureConfig.No))
            },

            {
                typeof(BgmConfig),
                AccessTools.Field(typeof(BgmConfig), nameof(BgmConfig.No))
            },

            {
                typeof(ExhibitConfig),
                AccessTools.Field(typeof(ExhibitConfig), nameof(ExhibitConfig.Index))
            },

            {
                typeof(JadeBoxConfig),
                AccessTools.Field(typeof(JadeBoxConfig), nameof(JadeBoxConfig.Index))
            },

            {
                // kind of an index
                typeof(PlayerUnitConfig),
                AccessTools.Field(typeof(PlayerUnitConfig), nameof(PlayerUnitConfig.ShowOrder))
            },
        };

        public static FieldInfo HasIndex(Type configType)
        {
            if (!GetAllConfigTypes().Contains(configType))
            {
                log.LogWarning($"HasIndex: {configType} is not a config type");
                return null;
            }
            if (indexFields.TryGetValue(configType, out var fieldInfo))
                return fieldInfo;
            return null;    
        }


        static string[] potentialArrayNames = new string[] { "_data" };

        static Dictionary<Type, FieldInfo> arrayFieldCache = new Dictionary<Type, FieldInfo>();

        public static FieldInfo GetArrayField(Type configType) => GetDataField(configType, potentialArrayNames, arrayFieldCache);

        static string[] potentialTableNames = new string[] { "_IdTable", "_LevelTable", "_NameTable", "_IDTable" };

        static Dictionary<Type, FieldInfo> tableFieldCache = new Dictionary<Type, FieldInfo>();

        public static FieldInfo GetTableField(Type configType) => GetDataField(configType, potentialTableNames, tableFieldCache);

        public static FieldInfo GetDataField(Type configType, string[] potentialNames, Dictionary<Type, FieldInfo> cache)
        {
            if (cache.TryGetValue(configType, out FieldInfo result))
                return result;

            if (!GetAllConfigTypes().Contains(configType))
            {
                log.LogWarning($"GetDataField: {configType} is not a config type");
                return null;
            }

            FieldInfo field = null;
            foreach (var n in potentialNames)
            {
                field =  AccessTools.Field(configType, n);
                if(field != null )
                { 
                    break ; 
                }
            }

            if (field == null)
            {
                throw new MissingMemberException($"None of the potential names managed to reflect a field from {configType}");
            }

            cache.Add(configType, field);

            return field;
        }

        static string[] potentialFromIdNames = new string[] { "FromId", "FromName", "FromLevel", "FromID" };

        static Dictionary<Type, MethodInfo> fromIdCache = new Dictionary<Type, MethodInfo>();

        public static MethodInfo GetFromIdMethod(Type configType, bool verify = true)
        {

            if (fromIdCache.TryGetValue(configType, out MethodInfo result))
                return result;

            if (!GetAllConfigTypes().Contains(configType))
            {
                log.LogWarning($"GetFromIdMethod: {configType} is not a config type");
                return null;
            }
            
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
                    throw new MissingMemberException($"{configType}: {mFromId.Name} did not have expected number of parameters. {parameters.Count()} instead of 1.");
                }
                if (!parameters.All(pi => pi.ParameterType == typeof(string)))
                {
                    throw new MissingMemberException($"{configType}: {mFromId.Name} not all parameters are string type");
                }
            }

            fromIdCache.Add(configType, mFromId);

            return mFromId;
        }

        public static IEnumerable<Type> GetAllConfigTypes(IEnumerable<Assembly> assemblies = null, bool refresh = false)
        {
            if (assemblies == null)
                assemblies = configAssemblies;

            if(refresh)
                configTypeCache.Clear();

            if (configTypeCache.Empty())
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

            var u = methods.Aggregate((s1, s2) => s1.Union(s2, new CompareByMethodName()).ToHashSet(new CompareByMethodName()));
            var v = methods.Aggregate((s1, s2) => s1.Intersect(s2, new CompareByMethodName()).ToHashSet(new CompareByMethodName()));

            log.LogInfo("----");

            u.Except(v, new CompareByMethodName()).Do(m => log.LogInfo($"{m.DeclaringType}: {m.Name}"));



        }


        public static void LogArrayNames() => LogFieldNames(AccessTools.Field(typeof(CardConfig), "_data").FieldType);

        public static void LogTableNames() => LogFieldNames(AccessTools.Field(typeof(CardConfig), "_IdTable").FieldType);


        public static void LogFieldNames(Type fieldType)
        {
            var types = GetAllConfigTypes();

            IEnumerable<HashSet<FieldInfo>> fieldSets = null;

 

            if (fieldType.IsGenericType)
            {
                fieldSets = types.Select(t => AccessTools.GetDeclaredFields(t).Where(f => f.FieldType.IsGenericType && f.FieldType.GetGenericTypeDefinition() == fieldType.GetGenericTypeDefinition()).ToHashSet(new CompareByFieldName()));
            }
            else 
            {
                fieldSets = types.Select(t => AccessTools.GetDeclaredFields(t).Where(f => f.FieldType == fieldType || f.FieldType.BaseType == fieldType.BaseType).ToHashSet(new CompareByFieldName()));
            }


                
            fieldSets.Do(h => h.Do(f => log.LogInfo($"{f.DeclaringType}: {f.Name}")));
            log.LogInfo("----");

            fieldSets.Aggregate((s1, s2) => s1.Union(s2, new CompareByFieldName()).ToHashSet(new CompareByFieldName())).Do(f => log.LogInfo($"{f.DeclaringType}: {f.Name}"));

            log.LogInfo("field names repeating only once: ");
            fieldSets.Aggregate((s1, s2) => s1.Union(s2, new CompareByFieldName()).Except(s1.Intersect(s2, new CompareByFieldName()), new CompareByFieldName()).ToHashSet(new CompareByFieldName())).
            Do(f => log.LogInfo($"{f.DeclaringType}: {f.Name}"));

        }


    }

    public class CompareByMethodName : IEqualityComparer<MethodInfo>
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

    public class CompareByFieldName : IEqualityComparer<FieldInfo>
    {
        public bool Equals(FieldInfo x, FieldInfo y)
        {
            return x.Name == y.Name;
        }

        public int GetHashCode(FieldInfo obj)
        {
            return obj.Name.GetHashCode();
        }
    }

}
