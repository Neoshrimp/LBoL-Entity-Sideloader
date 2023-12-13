using HarmonyLib;
using LBoLEntitySideloader.Utils;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace LBoLEntitySideloader
{
    internal class DeferredActions
    {
        internal Dictionary<IdContainer, Action> actions = new Dictionary<IdContainer, Action>();
        internal Dictionary<IdContainer, Action> reloadableActions = new Dictionary<IdContainer, Action>();

        internal void AddAction(IdContainer id, Action action, Assembly callingAssembly)
        {
            if(callingAssembly.IsLoadedFromDisk())
                actions.AlwaysAdd(id, action);
            else
                reloadableActions.Add(id, action);
        }

        internal void DoAll()
        {
            actions.Values.Do(a => a.Invoke());
            reloadableActions.Values.Do(a => a.Invoke());
        }

        internal void Clear()
        {
            reloadableActions.Clear();
        }

        internal void Reload()
        {
            reloadableActions.Values.Do(a => a.Invoke());
        }

    }
}
