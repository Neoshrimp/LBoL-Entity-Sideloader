using HarmonyLib;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Cards;
using LBoL.Core.Units;
using LBoL.EntityLib.EnemyUnits.Normal.Guihuos;
using LBoL.Presentation.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine.Profiling;
using static LBoLEntitySideloader.BepinexPlugin;

namespace LBoLEntitySideloader.CustomHandlers
{
    public delegate GameEvent<T> EventProvider<T, in PT>(PT provider) where T : GameEventArgs;


    public class CHandlerManager
    {

        public Dictionary<Type, HashSet<IHandleHolder>> gameRunHandlers = new Dictionary<Type, HashSet<IHandleHolder>>();

        public Dictionary<Type, HashSet<IHandleHolder>> battleEventHandlers = new Dictionary<Type, HashSet<IHandleHolder>>();


        public static void RegisterBattleEventHandler<T>(
            EventProvider<T, BattleController> eventProvider,
            GameEventHandler<T> handler,
            Predicate<BattleController> filter = null,
            GameEventPriority priority = GameEventPriority.ConfigDefault) where T : GameEventArgs
        {
            var hh = RegisterEventHandler(eventProvider, handler, priority, UniqueTracker.Instance.cHandlerManager. battleEventHandlers);
            hh.filter = filter;
        }


        public static void RegisterGameEventHandler<T>(
            EventProvider<T, GameRunController> eventProvider,
            GameEventHandler<T> handler,
            GameEventPriority priority = GameEventPriority.ConfigDefault) where T : GameEventArgs
        {
            RegisterEventHandler(eventProvider, handler, priority, UniqueTracker.Instance.cHandlerManager.gameRunHandlers);
        }

        internal static HandlerHolder<T, PT> RegisterEventHandler<T, PT>(
            EventProvider<T, PT> eventProvider,
            GameEventHandler<T> handler,
            GameEventPriority priority,
            Dictionary<Type, HashSet<IHandleHolder>> handlers) where T : GameEventArgs where PT : class
        {

            handlers.TryAdd(typeof(T), new HashSet<IHandleHolder>());

            var hh = new HandlerHolder<T, PT>()
            {
                eventProvider = eventProvider,
                handler = handler,
                priority = priority
            };

            if (!handlers[typeof(T)].Add(hh))
            {
/*                handlers[typeof(T)].TryGetValue(hh, out var oldHh);
                hh = (HandlerHolder<T, PT>)oldHh;*/
                log.LogWarning($"Custom handler (event holder: {typeof(PT)}, args: {typeof(T)}) was already registered thus not registered again.");
            }
            return hh;
        }

        internal static void IterateHandlers(
            Dictionary<Type, HashSet<IHandleHolder>> handlers,
            Action<IHandleHolder> hhAction,
            Func<Type, IHandleHolder, bool> filter = null)
        {

            foreach ((var key, var hset) in handlers)
            {
                foreach (var hh in hset)
                {
                    if (filter == null || filter(key, hh))
                        hhAction(hh);
                }
            }
        }
    }




  




}
