using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using LBoL.Base;
using LBoL.Base.Extensions;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Adventures;
using LBoL.Core.Attributes;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActionRecord;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Battle.Interactions;
using LBoL.Core.Cards;
using LBoL.Core.Dialogs;
using LBoL.Core.GapOptions;
using LBoL.Core.Helpers;
using LBoL.Core.Intentions;
using LBoL.Core.JadeBoxes;
using LBoL.Core.PlatformHandlers;
using LBoL.Core.Randoms;
using LBoL.Core.SaveData;
using LBoL.Core.Stations;
using LBoL.Core.Stats;
using LBoL.Core.StatusEffects;
using LBoL.Core.Units;
using LBoL.EntityLib.Adventures;
using LBoL.EntityLib.Adventures.Common;
using LBoL.EntityLib.Adventures.FirstPlace;
using LBoL.EntityLib.Adventures.Shared12;
using LBoL.EntityLib.Adventures.Shared23;
using LBoL.EntityLib.Adventures.Stage1;
using LBoL.EntityLib.Adventures.Stage2;
using LBoL.EntityLib.Adventures.Stage3;
using LBoL.EntityLib.Cards.Character.Cirno;
using LBoL.EntityLib.Cards.Character.Cirno.FairySupport;
using LBoL.EntityLib.Cards.Character.Koishi;
using LBoL.EntityLib.Cards.Character.Marisa;
using LBoL.EntityLib.Cards.Character.Reimu;
using LBoL.EntityLib.Cards.Character.Sakuya;
using LBoL.EntityLib.Cards.Devel;
using LBoL.EntityLib.Cards.Neutral;
using LBoL.EntityLib.Cards.Neutral.Black;
using LBoL.EntityLib.Cards.Neutral.Blue;
using LBoL.EntityLib.Cards.Neutral.Green;
using LBoL.EntityLib.Cards.Neutral.MultiColor;
using LBoL.EntityLib.Cards.Neutral.NoColor;
using LBoL.EntityLib.Cards.Neutral.Red;
using LBoL.EntityLib.Cards.Neutral.TwoColor;
using LBoL.EntityLib.Cards.Neutral.White;
using LBoL.EntityLib.Cards.Other.Adventure;
using LBoL.EntityLib.Cards.Other.Enemy;
using LBoL.EntityLib.Cards.Other.Misfortune;
using LBoL.EntityLib.Cards.Other.Tool;
using LBoL.EntityLib.Devel;
using LBoL.EntityLib.Dolls;
using LBoL.EntityLib.EnemyUnits.Character;
using LBoL.EntityLib.EnemyUnits.Character.DreamServants;
using LBoL.EntityLib.EnemyUnits.Lore;
using LBoL.EntityLib.EnemyUnits.Normal;
using LBoL.EntityLib.EnemyUnits.Normal.Bats;
using LBoL.EntityLib.EnemyUnits.Normal.Drones;
using LBoL.EntityLib.EnemyUnits.Normal.Guihuos;
using LBoL.EntityLib.EnemyUnits.Normal.Maoyus;
using LBoL.EntityLib.EnemyUnits.Normal.Ravens;
using LBoL.EntityLib.EnemyUnits.Opponent;
using LBoL.EntityLib.Exhibits;
using LBoL.EntityLib.Exhibits.Adventure;
using LBoL.EntityLib.Exhibits.Common;
using LBoL.EntityLib.Exhibits.Mythic;
using LBoL.EntityLib.Exhibits.Seija;
using LBoL.EntityLib.Exhibits.Shining;
using LBoL.EntityLib.JadeBoxes;
using LBoL.EntityLib.Mixins;
using LBoL.EntityLib.PlayerUnits;
using LBoL.EntityLib.Stages;
using LBoL.EntityLib.Stages.NormalStages;
using LBoL.EntityLib.StatusEffects.Basic;
using LBoL.EntityLib.StatusEffects.Cirno;
using LBoL.EntityLib.StatusEffects.Enemy;
using LBoL.EntityLib.StatusEffects.Enemy.SeijaItems;
using LBoL.EntityLib.StatusEffects.Marisa;
using LBoL.EntityLib.StatusEffects.Neutral;
using LBoL.EntityLib.StatusEffects.Neutral.Black;
using LBoL.EntityLib.StatusEffects.Neutral.Blue;
using LBoL.EntityLib.StatusEffects.Neutral.Green;
using LBoL.EntityLib.StatusEffects.Neutral.Red;
using LBoL.EntityLib.StatusEffects.Neutral.TwoColor;
using LBoL.EntityLib.StatusEffects.Neutral.White;
using LBoL.EntityLib.StatusEffects.Others;
using LBoL.EntityLib.StatusEffects.Reimu;
using LBoL.EntityLib.StatusEffects.Sakuya;
using LBoL.EntityLib.UltimateSkills;
using LBoL.Presentation;
using LBoL.Presentation.Animations;
using LBoL.Presentation.Bullet;
using LBoL.Presentation.Effect;
using LBoL.Presentation.I10N;
using LBoL.Presentation.UI;
using LBoL.Presentation.UI.Dialogs;
using LBoL.Presentation.UI.ExtraWidgets;
using LBoL.Presentation.UI.Panels;
using LBoL.Presentation.UI.Transitions;
using LBoL.Presentation.UI.Widgets;
using LBoL.Presentation.Units;
using LBoLEntitySideloader.Resource;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;
using Untitled;
using Untitled.ConfigDataBuilder;
using Untitled.ConfigDataBuilder.Base;
using YamlDotNet.RepresentationModel;
using static LBoLEntitySideloader.BepinexPlugin;
using Debug = UnityEngine.Debug;


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
        
        internal Assembly assembly;

        /// <summary>
        /// Returns as unique Id of the entity, should be used to . For now the result is the same as GetId().
        /// </summary>
        public IdContainer UniqueId
        {
            get 
            { 
                return UniqueTracker.GetUniqueId(this);
            }
        }

        /// <summary>
        /// Must return the Id of the entity. Id is the element which binds all entity components, logic, localization assets etc., together. There are two important requirements for an Id:
        /// First, it must be unique for its type, i.e. all cards must have unique Id but an exhibit could have the same Id as a card (as long as its unique among all exhibits).
        /// Second, if an entity has a logic component defining its behavior, the Id must be the same as that type's name. Most of the interesting entities have a logic component: cards have a concrete type extending Card, exhibit extending Exhibit and so on. It's best to use nameof(EntityLogic).
        /// This could result in an issue if another mod is happens to use the same type name as yours. Eventually, Sideloader might handle conflicting Ids but right now the game really expects the logic type name to be the same as the Id.
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
        /// Base entity logic Type (Card, Exhibit, EnemyUnit..) used by the template
        /// </summary>
        /// <returns>Type</returns>
        public abstract Type EntityType();


        internal void ProcessLocalization(LocalizationOption locOption, Action<string, Dictionary<string, object>> factoryAction)
        {
            if (locOption == null) return;

            var entityLogicType = EntityManager.Instance.sideloaderUsers.GetEntityLogicType(assembly, GetType());

            if (locOption is GlobalLocalization globalLoc)
            {
                var typesToLocalize = EntityManager.Instance.sideloaderUsers.userInfos[assembly].typesToLocalize;

                typesToLocalize.TryAdd(EntityType(), new LocalizationInfo());
                var locInfo = typesToLocalize[EntityType()];

                if (globalLoc.LocalizationFiles != null) 
                {
                    if (locInfo.locFiles == null)
                        locInfo.locFiles = globalLoc.LocalizationFiles;
                    else
                        Log.LogDev()?.LogWarning($"{assembly.GetName().Name}: {GetType()} tries to set global localization files again");
                }
                locInfo.entityLogicTypes.Add(entityLogicType);
                    
            }

            if (locOption is LocalizationFiles locFiles)
            {

                var termDic = locFiles.LoadLocTable(EntityType(), new Type[] { entityLogicType });


                if (termDic != null)
                {
                    foreach (var kv in termDic)
                    {
                        if (kv.Value.Empty())
                            LocalizationFiles.MissingValueError(kv.Key);

                        factoryAction(kv.Key, kv.Value);
                    }
                }
            }

        }

    }

}
