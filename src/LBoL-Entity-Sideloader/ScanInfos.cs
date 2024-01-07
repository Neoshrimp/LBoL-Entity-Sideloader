using BepInEx;
using HarmonyLib;
using LBoL.Base.Extensions;
using LBoLEntitySideloader.Attributes;
using LBoLEntitySideloader.Entities;
using LBoLEntitySideloader.ReflectionHelpers;
using LBoLEntitySideloader.Resource;
using LBoLEntitySideloader.TemplateGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace LBoLEntitySideloader
{
    public class UserInfo
    {
        public string GUID;

        public Assembly assembly;

        // 2do refactor

        /// <summary>
        /// EntityDefinition type => EntityDefinition instance
        /// </summary>
        public Dictionary<Type, EntityDefinition> definitionInfos = new Dictionary<Type, EntityDefinition>();

        /// <summary>
        /// entity factory type => concrete entity type
        /// </summary>
        public Dictionary<Type, List<EntityInfo>> entityInfos = new Dictionary<Type, List<EntityInfo>>();

        /// <summary>
        /// EntityDefinition type => entity logic type
        /// </summary>
        public Dictionary<Type, Type> definition2customEntityLogicType = new Dictionary<Type, Type>();

        /// <summary>
        /// definition type => attribute
        /// </summary>
        public Dictionary<Type, ModificationInfo> entitiesToOverwrite = new Dictionary<Type, ModificationInfo>();

        public Dictionary<string, Type> typeName2VanillaType = new Dictionary<string, Type>();


        public bool IsForOverwriting(Type definitionType)
        {
            return entitiesToOverwrite.ContainsKey(definitionType);
        }
    }

    public class EntityInfo
    {
        public Type factoryType;

        public Type entityType;

        public Type definitionType;

        // used when overwriting vanilla type
        public Type originalType;

        public EntityInfo(Type factoryType, Type entityType, Type definitionType)
        {
            this.factoryType = factoryType;
            this.entityType = entityType;
            this.definitionType = definitionType;
        }
    }

    public class LocalizationInfo
    {
        public LocalizationFiles locFiles;
       
        public HashSet<Type> entityLogicTypes = new HashSet<Type>();
    }

    public class ModificationInfo
    {
        public OverwriteVanilla attribute;
    }


    public class OverwriteInfo
    {

        public IdContainer entityId;

        public string componentName;

        public Type defType;

        public UserInfo user;

    }

}
