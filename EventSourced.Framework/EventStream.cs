using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using EventSourced.Framework.Abstractions;

namespace EventSourced.Framework
{
    public class EventStream : IEventStream
    {
        private Dictionary<Type, List<dynamic>> handlerList = new Dictionary<Type, List<dynamic>>();

        public void Subscribe<T>(EventHandlerCallback handler)
        {
            if (!handlerList.ContainsKey(typeof(T)))
                handlerList.Add(typeof(T), new List<dynamic>());

            handlerList[typeof(T)].Add(handler);
        }

        public void Publish(IEvent notification)
        {
            Task.Factory.StartNew(() => {
                var eventType = notification.Payload.GetType();

                if (!handlerList.ContainsKey(eventType))
                    return;

                var handlers = handlerList[eventType].AsReadOnly();
                foreach (var handler in handlers)
                {
                    ((Delegate)handler).DynamicInvoke(notification.PersistenceId, notification.Position, notification.Payload);
                }
            });
        }
    }
}
