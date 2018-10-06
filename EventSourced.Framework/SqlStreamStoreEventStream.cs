using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SqlStreamStore;
using SqlStreamStore.Streams;
using SqlStreamStore.Subscriptions;

namespace Lab.SqlStreamStoreDemo.Framework
{
    public class SqlStreamStoreEventStream : IEventStream
    {
        private readonly IStreamStore streamStore;
        private Dictionary<Type, List<dynamic>> handlerList = new Dictionary<Type, List<dynamic>>();

        public SqlStreamStoreEventStream(IStreamStore streamStore)
        {
            this.streamStore = streamStore;

            streamStore.SubscribeToAll(null, AllStreamMessageReceived, AllSubscriptionDropped);
        }

         public void Subscribe<T>(Action<T> handler)
        {
            if (!handlerList.ContainsKey(typeof(T)))
                handlerList.Add(typeof(T), new List<dynamic>());

            handlerList[typeof(T)].Add(handler);
        }

        public void Publish(object @event)
        {
            var eventType =  @event.GetType();

            if (!handlerList.ContainsKey(eventType))
                return;

            var handlers = handlerList[ @event.GetType()];
            foreach (var handler in handlers)
            {
                ((Delegate)handler).DynamicInvoke( @event);
            }

            return;
        }

        private async Task AllStreamMessageReceived(IAllStreamSubscription subscription, StreamMessage streamMessage, CancellationToken cancellationToken)
        {
            var eventJson = await streamMessage.GetJsonData();
            var eventTypeName = $"Lab.SqlStreamStoreDemo.ExampleAggregate.Events.{streamMessage.Type}, EventSourced.Example";
            var eventType = Type.GetType(eventTypeName);
            var @event = JsonConvert.DeserializeObject(eventJson, eventType);

            Publish(@event);
        }

        private void AllSubscriptionDropped(IAllStreamSubscription subscription, SubscriptionDroppedReason reason, Exception exception = null)
        {
        }

        

       
    }
}