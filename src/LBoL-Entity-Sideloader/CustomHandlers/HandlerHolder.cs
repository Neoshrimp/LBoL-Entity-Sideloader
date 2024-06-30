using LBoL.Core;
using System;

namespace LBoLEntitySideloader.CustomHandlers
{

    public interface IHandleHolder
    {
        void RegisterHandler(object eventProvider);

        void UnregisterHandler(object eventProvider);
    }

    public class HandlerHolder<T, PT> : IHandleHolder where T : GameEventArgs where PT : class
    {


        public EventProvider<T, PT> eventProvider;

        public GameEventHandler<T> handler;

        public GameEventPriority priority;

        public Predicate<PT> filter;

        public override bool Equals(object obj)
        {
            if (obj is HandlerHolder<T, PT> otherHh)
                return (handler?.Equals(otherHh.handler) ?? otherHh.handler == null)
                       && (filter?.Equals(otherHh.filter) ?? otherHh.filter == null)
                       && priority == otherHh.priority;
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(handler, filter, priority);
        }

        public GameEventPriority GetPriority()
        {
            return priority;
        }

        public void RegisterHandler(object provider)
        {
            if (provider is PT castProvider)
            {
                if (filter == null || filter(castProvider))
                {
                    var _event = eventProvider.Invoke(castProvider);
                    _event.AddHandler(handler, priority);
                }
            }
        }

        public void UnregisterHandler(object provider)
        {
            if (provider is PT castProvider)
            {

                var _event = eventProvider.Invoke(castProvider);
                _event.RemoveHandler(handler, priority);
            }
        }
    }




}
