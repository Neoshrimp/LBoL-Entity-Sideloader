﻿using HarmonyLib;
using LBoL.Base.Extensions;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Adventures;
using LBoL.Core.Cards;
using LBoL.Core.GapOptions;
using LBoL.Core.StatusEffects;
using LBoL.Core.Units;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Collections;
using UnityEngine;
using System.Linq;
using System.IO;
using System.Text;

namespace LBoLEntitySideloader.ReflectionHelpers
{

    public class TypeFactoryReflection 
    {
        private static readonly BepInEx.Logging.ManualLogSource log = BepinexPlugin.log;


        // from Library.RegisterAllAsync. No sensible common pattern exists to extract these types reflectively
        public static HashSet<Type> factoryTypes = new HashSet<Type>() { typeof(Stage), typeof(Card), typeof(UltimateSkill), typeof(Exhibit), typeof(PlayerUnit), typeof(EnemyUnit), typeof(StatusEffect), typeof(Doll), typeof(GapOption), typeof(Intention), typeof(Adventure), typeof(JadeBox) };


        static Type genericTypeFactoryType = typeof(TypeFactory<>).GetGenericTypeDefinition();

        static Dictionary<Type, Dictionary<TableFieldName, AccessTools.FieldRef<object, Dictionary<string, Type>>>> entityDicCache = new Dictionary<Type, Dictionary<TableFieldName, AccessTools.FieldRef<object, Dictionary<string, Type>>>>();

        public static AccessTools.FieldRef<object, Dictionary<string, Type>> AccessTypeDicts(Type facType, TableFieldName tableFieldName)
        {

            if (!factoryTypes.Contains(facType))
            {
                log.LogWarning($"GetAccessRef: {facType} is not a type used by TypeFactory");
                return null;
            }


            if (entityDicCache.TryGetValue(facType, out Dictionary<TableFieldName, AccessTools.FieldRef<object, Dictionary<string, Type>>> potentialResult))
            {
                if (potentialResult.TryGetValue(tableFieldName, out AccessTools.FieldRef<object, Dictionary<string, Type>> result))
                { 
                    return result;
                }
            }
            var typeFactoryType = genericTypeFactoryType.MakeGenericType(new Type[] {facType});

            var fieldRef = AccessTools.FieldRefAccess<Dictionary<string, Type>>(typeFactoryType, tableFieldName.ToString());


            entityDicCache.TryAdd(facType, new Dictionary<TableFieldName, AccessTools.FieldRef<object, Dictionary<string, Type>>>());
            entityDicCache[facType].TryAdd(tableFieldName, fieldRef);

            return fieldRef;
        
        }

        public enum TableFieldName
        {
            FullNameTypeDict,
            TypeDict

        }

        static Dictionary<Type, AccessTools.FieldRef<object, Dictionary<string, Dictionary<string, object>>>> typeLocalizerCache = new Dictionary<Type, AccessTools.FieldRef<object, Dictionary<string, Dictionary<string, object>>>>();


        public static AccessTools.FieldRef<object, Dictionary<string, Dictionary<string, object>>> AccessTypeLocalizers(Type facType)
        {
            if (!factoryTypes.Contains(facType))
            {
                log.LogWarning($"AccessTypeLocalizers: {facType} is not a type used by TypeFactory");
                return null;
            }

            if (typeLocalizerCache.TryGetValue(facType, out AccessTools.FieldRef<object, Dictionary<string, Dictionary<string, object>>> result))
            {
                return result;
            }


            var typeFactoryType = genericTypeFactoryType.MakeGenericType(new Type[] { facType });


            var fieldRef = AccessTools.FieldRefAccess<Dictionary<string, Dictionary<string, object>>> (typeFactoryType, nameof(TypeFactory<object>._typeLocalizers));

            typeLocalizerCache.TryAdd(facType, fieldRef);
            return fieldRef;


        }

        // close enough
        static public void OutputVirtualMembers()
        {
            var types = new HashSet<Type>();

            factoryTypes.Do(t =>
            {
                types.Add(t);
                var tempT = t;
                while (tempT != typeof(object))
                {
                    types.Add(tempT);
                    tempT = tempT.BaseType;

                }
            });

            var output = new List<MemberInfo>();

            types.Do(t => {

                AccessTools.GetDeclaredMethods(t).Do(m =>
                {
                    if (m.IsVirtual)
                        output.Add(m);
                });
            });

            using (FileStream fileStream = File.Open("VirtualMembers.txt", FileMode.Create, FileAccess.Write, FileShare.None))
            {
                using (StreamWriter streamWriter = new StreamWriter(fileStream, Encoding.UTF8))
                {
                    foreach (var m in output)
                    {
                        streamWriter.WriteLine($"<DoNotPublicize Include=\"{m.DeclaringType.Assembly.GetName().Name}:{m.DeclaringType.FullName}.{m.Name}\"/>");
                    }
                }
            }
        }

    }




    class AssignabilityComparer : IEqualityComparer<Type>
    {
        public bool Equals(Type x, Type y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }

            if (x == null || y == null)
            {
                return false;
            }

            return x.IsSubclassOf(y) || y.IsSubclassOf(x);
        }

        public int GetHashCode(Type obj)
        {
            return obj == null ? 0 : obj.GetHashCode();
        }
    }


}
