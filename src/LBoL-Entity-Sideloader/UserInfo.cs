using BepInEx;
using HarmonyLib;
using LBoL.Base.Extensions;
using LBoLEntitySideloader.Attributes;
using LBoLEntitySideloader.Entities;
using LBoLEntitySideloader.ReflectionHelpers;
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
        public Dictionary<Type, Type> definition2EntityType = new Dictionary<Type, Type>();

        // definition => id
        public Dictionary<Type, string> entitiesToModify = new Dictionary<Type, string>();


    }

    public class EntityInfo
    {
        public Type factoryType;

        public Type entityType;

        public Type definitionType;

        public EntityInfo(Type factoryType, Type entityType, Type definitionType)
        {
            this.factoryType = factoryType;
            this.entityType = entityType;
            this.definitionType = definitionType;
        }
    }

}
