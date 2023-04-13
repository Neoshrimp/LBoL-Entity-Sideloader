using LBoL.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace LBoLEntitySideloader
{
    internal class dummy<T> where T : class
    {

        internal static string LocalizeProperty(string id, string key, bool decorated, bool required)
        {
            Dictionary<string, object> dictionary;
            object obj;
            if (TypeFactory<T>._typeLocalizers.TryGetValue(id, out dictionary) && dictionary.TryGetValue(key, out obj))
            {
                string text = obj as string;
                if (text == null)
                {
                    string text2;
                    if (!TypeFactory<T>._failureTable.TryGetValue(id + "." + key, out text2))
                    {
                        text2 = string.Concat(new string[] { "<", id, ".", key, ">(Error)" });
                        TypeFactory<T>._failureTable.Add(id + "." + key, text2);

                    }
                    return text2;
                }
                if (!decorated)
                {
                    return text;
                }
                return StringDecorator.Decorate(text);
            }
            else
            {
                if (required)
                {
                    string text3;
                    if (!TypeFactory<T>._failureTable.TryGetValue(id + "." + key, out text3))
                    {
                        text3 = string.Concat(new string[] { "<", id, ".", key, ">" });
                        TypeFactory<T>._failureTable.Add(id + "." + key, text3);
                    }
                    return text3;
                }
                return null;
            }
        }
    }
}
