﻿using HarmonyLib;
using LBoLEntitySideloader.ReflectionHelpers;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace GunDesigner.ConfigBuilders
{

    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public sealed class ConfigFieldBindAttribute : Attribute
    {
        private readonly Type configType;
        private readonly string fieldName;

        public ConfigFieldBindAttribute(Type configType, string fieldName, bool backFieldName = true)
        {
            this.configType = configType;
            this.fieldName = fieldName;

            if (backFieldName)
                this.fieldName = ConfigReflection.BackingWrap(fieldName);
        }


        public FieldInfo TryGetFieldRef()
        {

            var field = AccessTools.Field(configType, fieldName);
            if (field == null)
                throw new MissingFieldException($"Could not find field {fieldName} in {configType.Name}");
            return field;
        }

    }
}
