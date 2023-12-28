using LBoL.Base.Extensions;
using LBoL.Core;
using LBoL.Core.Adventures;
using LBoLEntitySideloader.Resource;
using System;
using System.Collections.Generic;
using System.Reflection;


namespace LBoLEntitySideloader.Entities
{

    public interface IConfigProvider<out C> where C : class
    {
        abstract public C DefaultConfig();
        abstract public C MakeConfig();
    }

    public interface ITypeProvider<T> where T : class { }

    public interface IGameEntityProvider<E> : ITypeProvider<E> where E : GameEntity { }

    // Adventure does not extend GameEntity
    public interface IAdventureProvider<A> : ITypeProvider<A> where A : Adventure { }


    public abstract class EntityDefinition
    {
        
        public Assembly userAssembly;
        public UserInfo user;



        /// <summary>
        /// Returns as unique Id of the entity, should be used to when referring to the entity in your own code. For now the result is the same as GetId().
        /// </summary>
        public IdContainer UniqueId
        {
            get 
            { 
                return UniqueTracker.GetUniqueId(this);
            }
        }



        /// <summary>
        /// Must return the Id of the entity. Id is the element which binds all entity components, logic, localization, assets etc., together. There are two important requirements for an Id:
        /// First, it must be unique for its type, i.e. all cards must have unique Id but an exhibit could have the same Id as a card (as long as its unique among all exhibits).
        /// Second, if an entity has a logic component defining its behavior, the Id must be the same as that type's name. Most of the interesting entities have a logic component: cards have a concrete type extending Card, exhibit extending Exhibit and so on. It's best to use nameof(EntityLogic).
        /// This could result in an issue if another mod happens to use the same type name as yours. Eventually, the Sideloader might handle conflicting Ids but right now the game really expects the logic type name to be the same as the Id.
        /// GetId() should never be used when referring to the entity in your own code, for example, when specifying RelativeCards config property. UniqueId should be used instead. However, GetId() can and should be used when referring to file names of resources as UniqueId can vary depending on Id conflicts.
        /// If a definition is overwriting a vanilla entity, the GetId is used to specify which entity to overwrite by returning the Id of the entity being overwritten.
        /// </summary>
        /// <returns>IdContainer but currently it should just return a string (which will get implicitly converted to IdContainer)</returns>
        public abstract IdContainer GetId();

        /// <summary>
        /// Config Type used by the template
        /// </summary>
        /// <returns>Type</returns>
        public abstract Type ConfigType();

        /// <summary>
        /// Base template class type
        /// </summary>
        /// <returns>Type</returns>
        public abstract Type TemplateType();



        /// <summary>
        /// Base entity logic Type (Card, Exhibit, EnemyUnit..) used by the template
        /// </summary>
        /// <returns>Type</returns>
        public abstract Type EntityType();




        internal void ProcessLocalization(LocalizationOption locOption, Type facType)
        {
            if (locOption == null) return;

            var entityLogicType = SideloaderUsers.GetEntityLogicType(userAssembly, GetType());

            if (locOption is GlobalLocalization globalLoc)
            {

                UniqueTracker.Instance.typesToLocalize.TryAdd(userAssembly, new Dictionary<Type, LocalizationInfo>());
                var typesToLocalize = UniqueTracker.Instance.typesToLocalize[userAssembly];

                typesToLocalize.TryAdd(EntityType(), new LocalizationInfo());
                var locInfo = typesToLocalize[EntityType()];

                if (globalLoc.LocalizationFiles.locTable.NotEmpty()) 
                {
                    if (locInfo.locFiles == null)
                        locInfo.locFiles = globalLoc.LocalizationFiles;
                    else
                        Log.LogDevExtra()?.LogWarning($"{userAssembly.GetName().Name}: {GetType()} tries to set global localization files but they've already been set by another {TemplateType().Name}.");
                }
                locInfo.entityLogicTypes.Add(entityLogicType);
                    
            }

            if (locOption is LocalizationFiles locFiles)
            {
                var termDic = locFiles.LoadLocTable(EntityType(), new Type[] { entityLogicType });
                LocalizationOption.FillLocalizationTables(termDic, facType, locFiles.mergeTerms);
            }

            if (locOption is DirectLocalization rawLoc)
            {

                var termDic = rawLoc.WrapTermDic(UniqueId);

                LocalizationOption.FillLocalizationTables(termDic, facType, rawLoc.mergeTerms);

            }

        }

    }

}
