using BepInEx;
using HarmonyLib;
using LBoL.Base.Extensions;
using LBoLEntitySideloader.Attributes;
using LBoLEntitySideloader.Entities;
using LBoLEntitySideloader.ReflectionHelpers;
using LBoLEntitySideloader.Resources;
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

        // EntityDefinition type => EntityDefinition
        public Dictionary<Type, EntityDefinition> definitionInfos = new Dictionary<Type, EntityDefinition>();

        // entity factory type => concrete entity type
        public Dictionary<Type, List<EntityInfo>> entityInfos = new Dictionary<Type, List<EntityInfo>>();

        // EntityDefinition type => entity type
        public Dictionary<Type, Type> definition2EntityLogicType = new Dictionary<Type, Type>();

        // definition type => attribute
        public Dictionary<Type, ModificationInfo> entitiesToOverwrite = new Dictionary<Type, ModificationInfo>();

        public Dictionary<string, Type> typeName2VanillaType = new Dictionary<string, Type>();


        public Dictionary<Type, LocalizationInfo> typesToLocalize = new Dictionary<Type, LocalizationInfo>();

        public bool IsForOverwriting(Type definitinoType)
        {
            return entitiesToOverwrite.ContainsKey(definitinoType);
        }

        public void ClearTypeToLocalize() 
        {
            typesToLocalize = new Dictionary<Type, LocalizationInfo>();
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

}
