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
using Debug = UnityEngine.Debug;

namespace LBoLEntitySideloader
{
    public class EntityManager
    {
        static private EntityManager _instance;

        static private BepInEx.Logging.ManualLogSource log = BepinexPlugin.log;

        public static EntityManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new EntityManager();
                return _instance;
            }
        }

        internal class SideloaderUsers
        {
            public Dictionary<Assembly, List<Type>> users = new Dictionary<Assembly, List<Type>>();

            public void AddUser(Assembly assembly)
            {
                if (users.ContainsKey(assembly))
                {
                    throw new Exception($"{assembly.GetName().Name} is already registered");
                
                }
                users.Add(assembly, FindEntityDefinitions(assembly));
            }

            public void RemoveUser(Assembly assembly)
            {
                if (!users.ContainsKey(assembly))
                {
                    throw new Exception($"{assembly.GetName().Name} is not registered");
                }

                users.Remove(assembly);
            }

            internal List<Type> FindEntityDefinitions(Assembly assembly)
            {
                // 2do add optional DontLoad attribute filter
                return assembly.GetExportedTypes().
                    Where(t => t.IsSubclassOfGeneric(typeof(EntityDefinition<,>))).ToList();
            }

        }

        internal SideloaderUsers sideloaderUsers;
        

        static public void RegisterSelf()
        {
            var a = Assembly.GetCallingAssembly();
            Instance.sideloaderUsers.AddUser(a);

        }




                
        static string[] potentialFromIdNames = new string[] { "FromId", "FromName", "FromLevel", "FromID" };
        
        internal void RegisterEntity<T, C>(EntityDefinition<T, C> entityDefinition) where T : class where C : class
        {

            log.LogInfo($"{entityDefinition.Id}, T:{typeof(T)}, C:{typeof(C)}");

            try
            {
                var cType = typeof(C);

                MethodInfo mFromId = null;

                foreach (var n in potentialFromIdNames)
                {
                    mFromId = AccessTools.Method(cType, n);
                    if (mFromId != null)
                    {
                        break;
                    }
                }

                if (mFromId == null)
                {
                    throw new MissingMemberException($"None of the potential fromId names managed to reflect a method from {cType}");
                }

                var config = (C)mFromId.Invoke(null, new object[] { entityDefinition.Id });
                var newConfig = entityDefinition.GetConfig();

                if (config == null)
                {
                    log.LogInfo($"initial config load for {entityDefinition.Id}");

                    var f_Data = AccessTools.Field(typeof(C), "_data");
                    var ref_Data = AccessTools.StaticFieldRefAccess<C[]>(f_Data);
                    var f_IdTable = AccessTools.Field(cType, "_IdTable");

                    ref_Data() = ref_Data().AddItem(newConfig).ToArray();
                    ((Dictionary<string, C>)f_IdTable.GetValue(null)).Add(entityDefinition.Id, newConfig);

                }
                else
                {
                    log.LogInfo($"secondary config reload for {entityDefinition.Id}");
                    config = newConfig;
                }

                if (TypeFactory<T>.TryGetType(entityDefinition.Id) == null)
                {
                    log.LogInfo($"registering public sealed types in {entityDefinition.Assembly}");
                    TypeFactory<T>.RegisterAssembly(entityDefinition.Assembly);
                }

            }
            catch (Exception ex)
            {

                log.LogError($"Exception registering {entityDefinition.Id}: {ex}");

            }
        }

        internal void RegisterUsers()
        {
            foreach (var kv in sideloaderUsers.users)
            {
                foreach (var def in kv.Value)
                {

                    // 2do sort this shit out
                    if (def is Type)
                    {
                       Activator.CreateInstance(def);
                    }
                }
            }
        }



    }
}
