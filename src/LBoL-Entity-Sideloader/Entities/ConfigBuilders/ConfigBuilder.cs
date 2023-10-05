using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using HarmonyLib;
using LBoL.Base.Extensions;
using LBoL.ConfigData;
using LBoLEntitySideloader.Entities.ConfigBuilders.Converters;

namespace LBoLEntitySideloader.Entities.ConfigBuilders
{
    public abstract class ConfigBuilder<C, RC> where C: class where RC : ReadableConfig<C>, new()
    {
        protected List<Type> converterTypes = new List<Type>();

        protected ConverterContainer converterContainer;

        public virtual C BuildConfig(RC readableConfig)
        {
            if (converterTypes.Empty())
            {
                converterTypes = converterContainer.GetType().GetNestedTypes(AccessTools.allDeclared).Where(t => t.GetCustomAttribute<ConfigFieldBindAttribute>() != null).ToList();
            }
                

            var config = (C)FormatterServices.GetUninitializedObject(typeof(C));
            foreach (var conType in converterTypes)
            {
                try
                {
                    var configFieldBind = conType.GetCustomAttribute<ConfigFieldBindAttribute>();
                    var readableFieldBind = conType.GetCustomAttribute<ReadableFieldBindAttribute>();

                    var con = AccessTools.Constructor(conType, new Type[] { }).Invoke(new object[] { });
                    var m_convert = AccessTools.Method(conType, nameof(IConverter<object, object>.ConvertTo));

                    var convertedVal = m_convert.Invoke(con, new object[] { readableFieldBind.TryGetFieldRef().GetValue(readableConfig) });

                    configFieldBind.TryGetFieldRef().SetValue(config, convertedVal);
                }
                catch (Exception ex)
                {
                    Log.log.LogError($"ConfigBuilder: {ex}");
                }
            }

            return config;
        }



        public virtual RC Config2ReadableConfig(C config) 
        {
            if (converterTypes.Empty())
            {
                converterTypes = converterContainer.GetType().GetNestedTypes(AccessTools.allDeclared).Where(t => t.GetCustomAttribute<ConfigFieldBindAttribute>() != null).ToList();
            }


            var readableConfig = new RC();
            foreach (var conType in converterTypes)
            {
                try
                {
                    var configFieldBind = conType.GetCustomAttribute<ConfigFieldBindAttribute>();
                    var readableFieldBind = conType.GetCustomAttribute<ReadableFieldBindAttribute>();

                    var con = AccessTools.Constructor(conType, new Type[] { }).Invoke(new object[] { });
                    var m_convert = AccessTools.Method(conType, nameof(ITwoWayConverter<object, object>.ReverseConvert));

                    var convertedVal = m_convert.Invoke(con, new object[] { configFieldBind.TryGetFieldRef().GetValue(config) });

                    readableFieldBind.TryGetFieldRef().SetValue(readableConfig, convertedVal);
                }
                catch (Exception ex)
                {
                    Log.log.LogError($"ConfigBuilder: {ex}");
                }
            }

            return readableConfig;
        }

    }
}
