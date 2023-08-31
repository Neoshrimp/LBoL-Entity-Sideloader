using LBoL.EntityLib.Exhibits.Shining;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace LBoLEntitySideloader.TemplateGen
{
    public class MethodCache
    {


        // templateType +=> definition type fullname +=> func name +=> Func<>
        public Dictionary<Type, Dictionary<string, Dictionary<string, Delegate>>> methodPayloadCache = new Dictionary<Type, Dictionary<string, Dictionary<string, Delegate>>>();



        public Delegate GetMethod(Type templateType, string defName, string name)
        {
            if (methodPayloadCache.TryGetValue(templateType, out var defDic))
            { 
                if(defDic.TryGetValue(defName, out var funcDic))
                    if(funcDic.TryGetValue(name, out var func))
                        return func;
            }

            return null;
        }

        public bool AddMethod(Type templateType, string defFullName, string name, Delegate func)
        {
            methodPayloadCache.TryAdd(templateType, new Dictionary<string, Dictionary<string, Delegate>>());
            methodPayloadCache[templateType].TryAdd(defFullName, new Dictionary<string, Delegate>());

            return methodPayloadCache[templateType][defFullName].TryAdd(name, func);
        }

        public void CleargenTemplateMethodPayloadCache()
        {
            methodPayloadCache = new Dictionary<Type, Dictionary<string, Dictionary<string, Delegate>>>();
        }



    }
}
