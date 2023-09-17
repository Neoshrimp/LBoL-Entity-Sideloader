using HarmonyLib;
using LBoL.Base.Extensions;
using LBoLEntitySideloader.Entities;
using LBoLEntitySideloader.ReflectionHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LBoLEntitySideloader
{
    public class TemplateSequenceTable
    {
        private static readonly BepInEx.Logging.ManualLogSource log = BepinexPlugin.log;

        private static Dictionary<Type, string> lookUpDic = new Dictionary<Type, string>();

        private Dictionary<string, Sequence> table = new Dictionary<string, Sequence>();

        public TemplateSequenceTable(int startingPoint = 0) 
        {
            if (lookUpDic.Empty())
            {

                foreach (var configType in ConfigReflection.AllConfigTypes())
                {
                    lookUpDic.Add(configType, configType.Name);
                }

                foreach (var tt in TemplatesReflection.AllTemplateTypes())
                {

                    // mangled name lmfao
                    var configType = tt.GetInterface($"{nameof(IConfigProvider<object>)}`1")?.GenericTypeArguments[0];
                    if (configType != null)
                    {
                        //lookUpDic.TryAdd(configType, configType.Name);
                        lookUpDic.Add(tt, configType.Name);
                    }
                    else
                    { 
                        // special case when template doesn't provide a config
                        lookUpDic.Add(tt, tt.Name);
                    }


                }

            }

            foreach (var kv in lookUpDic)
            {
                table.TryAdd(kv.Value, new Sequence(startingPoint));
            }
               
        }


        public Sequence Sequence(Type type)
        {
            TestType(type);
            return table[lookUpDic[type]];
        }

        public int Next(Type type)
        {
            TestType(type);
            return table[lookUpDic[type]].Next();
        }

        private void TestType(Type type)
        {
            if (!TemplatesReflection.AllTemplateTypes().Contains(type) && !ConfigReflection.AllConfigTypes().Contains(type))
            {
                throw new ArgumentException($"TemplateSequenceTable: {type} is not a Entity definition template type or a Config type");
            }

            if (!lookUpDic.ContainsKey(type))
            {
                throw new ArgumentException($"TemplateSequenceTable: {type} is not in lookup dictionary. Some templates might not have an associated config type. Try using template type instead.");

            }
        }
    }
}
