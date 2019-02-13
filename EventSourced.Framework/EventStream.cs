using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventSourced.Framework.Abstracions;

namespace EventSourced.Framework
{
    public class EventStream : IEventStream
    {
        private Dictionary<Type, List<dynamic>> handlerList = new Dictionary<Type, List<dynamic>>();

         public void Subscribe<T>(Action<string, T> handler)
        {
            if (!handlerList.ContainsKey(typeof(T)))
                handlerList.Add(typeof(T), new List<dynamic>());

            handlerList[typeof(T)].Add(handler);
        }

        public void Publish(string persistenceId, object notification)
        {
            Task.Factory.StartNew(() => {
                var eventType = notification.GetType();

                if (!handlerList.ContainsKey(eventType))
                    return;

                var handlers = handlerList[notification.GetType()].AsReadOnly();
                foreach (var handler in handlers)
                {
                    ((Delegate)handler).DynamicInvoke(persistenceId, notification);
                }
            });
        }
    }
}
