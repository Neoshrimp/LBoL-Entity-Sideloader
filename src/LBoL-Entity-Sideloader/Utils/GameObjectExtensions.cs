using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace LBoLEntitySideloader.Utils
{
    public static class GameObjectExtensions
    {
        /// <summary>
        /// Includes root object
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>
        public static IEnumerable<GameObject> IterateHierarchy(this Transform root)
        {
            Queue<Transform> queue = new Queue<Transform>();
            queue.Enqueue(root);

            while (queue.Count > 0)
            {
                Transform current = queue.Dequeue();
                yield return current.gameObject;

                foreach (Transform child in current)
                {
                    queue.Enqueue(child);
                }
            }
        }

        public static T JankyCopyComponent<T>(this GameObject destination, T original) where T : Component
        {
            System.Type type = original.GetType();
            Component copy = destination.AddComponent(type);
            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Default | BindingFlags.DeclaredOnly;
            PropertyInfo[] properties = type.GetProperties(bindingFlags);
            FieldInfo[] fields = type.GetFields(bindingFlags);

            foreach (var property in properties)
            {
                if (property.CanWrite)
                {
                    try
                    {
                        property.SetValue(copy, property.GetValue(original, null), null);
                    }
                    catch (Exception ex)
                    {
                        Log.LogDev()?.LogWarning($"Exception while copying  gameObject {destination.name}, component {typeof(T)}, property {property.Name}: {ex}");
                    }
                }
            }

            foreach (var field in fields)
            {
                field.SetValue(copy, field.GetValue(original));
            }
            return copy as T;
        }
    }
}
