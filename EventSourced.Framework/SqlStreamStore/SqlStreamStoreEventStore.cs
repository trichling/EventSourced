using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventSourced.Framework.Abstractions;
using Newtonsoft.Json;
using SqlStreamStore;
using SqlStreamStore.Streams;
using SqlStreamStore.Subscriptions;

namespace EventSourced.Framework.SqlStreamStore
{

    public class SqlStreamStoreEventStore : IEventStore
    {
        private readonly IStreamStore streamStore;
        private readonly ITypeResovler typeResovler;

        public SqlStreamStoreEventStore(IStreamStore streamStore, ITypeResovler typeResovler)
        {
            this.streamStore = streamStore;
            this.typeResovler = typeResovler;
        }

        public long StoreVersion()
        {
            var headPosition = streamStore.ReadHeadPosition().GetAwaiter().GetResult();
            return headPosition;
        }

        public async Task<IEnumerable<dynamic>> GetHistory(string persistenceId)
        {
            var history = new List<dynamic>();
            var streamId = new StreamId(persistenceId);
            var readStreamPage = await streamStore.ReadStreamForwards(streamId, global::SqlStreamStore.Streams.StreamVersion.Start, int.MaxValue);

            foreach (var message in readStreamPage.Messages)
            {
                var @event = await DeserializeEvent(message);
                history.Add(@event);
            }

            return history;
        }

        public async Task<long> Persist(string persistenceId, object @event)
        {
            var streamId = new StreamId(persistenceId);
            var message = new NewStreamMessage(Guid.NewGuid(), @event.GetType().Name, JsonConvert.SerializeObject(@event));
            var appendResult = await streamStore.AppendToStream(streamId, ExpectedVersion.Any, message);

            return appendResult.CurrentPosition;
        }

        public IDisposable CatchUpSubscription(long lastPosition, EventHandlerCallback onEvent, Action onHasCaughtUp)
        {
            if (StoreVersion() == -1)
                hasCaughtUpHandler(true);

            return this.streamStore.SubscribeToAll(lastPosition, messageHandler, subscriptionDroppedHandler, hasCaughtUpHandler);

            async Task messageHandler(IAllStreamSubscription subscription, StreamMessage streamMessage, CancellationToken cancellationToken) 
            {
                var @event = await DeserializeEvent(streamMessage);
                onEvent(streamMessage.StreamId, streamMessage.Position, @event);
            }

            void subscriptionDroppedHandler(IAllStreamSubscription subscription, SubscriptionDroppedReason reason, Exception exception = null)
            {
            }

            void hasCaughtUpHandler(bool hasCaughtUp)
            {
                if (hasCaughtUp)
                    onHasCaughtUp();
            }
        }

        private async Task<dynamic> DeserializeEvent(StreamMessage message)
        {
            var eventJson = await message.GetJsonData();
            var eventType = typeResovler.ResolveFrom(message.Type);
            var @event = JsonConvert.DeserializeObject(eventJson, eventType);

            return @event;
        }
    }
}