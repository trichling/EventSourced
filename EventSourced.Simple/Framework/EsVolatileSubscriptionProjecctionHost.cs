using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EventStore.Client;
using Newtonsoft.Json;

namespace EventSourced.Simple.Framework
{

    public class EsVolatileSubscriptionProjecctionHost : IProjectionHost
    {
        private readonly IProjection projection;
        private readonly EventStoreClient client;
        private readonly ICheckpointProvider checkpointProvider;
        private StreamSubscription subscription;

        public EsVolatileSubscriptionProjecctionHost(IProjection projection, EventStoreClient client, ICheckpointProvider checkpointProvider)
        {
            this.projection = projection;
            this.client = client;
            this.checkpointProvider = checkpointProvider;
        }

        public async Task Subscribe()
        {
            if (IsAllStream(projection.Stream))
                await SubscribeToAll();
            else
                await SubscribeToStream();
        }

        private async Task SubscribeToAll()
        {
            var checkpoint = checkpointProvider.Get();
            var position = checkpoint.HasValue ? new Position(checkpoint.Value, checkpoint.Value) : Position.Start;
            subscription = await client.SubscribeToAllAsync(position, OnEvent, subscriptionDropped: OnSubscriptionDropped);
        }

        private async Task SubscribeToStream()
        {
            var checkpoint = checkpointProvider.Get();

            if (checkpoint.HasValue)
                subscription = await client.SubscribeToStreamAsync(projection.Stream, new StreamPosition(checkpoint.Value), OnEvent, true, OnSubscriptionDropped);
            else
                subscription = await client.SubscribeToStreamAsync(projection.Stream, OnEvent, true, OnSubscriptionDropped);
        }

        public void Unsubscribe()
        {
            subscription.Dispose();
        }

        private async Task OnEvent(StreamSubscription _, ResolvedEvent resolvedEvent, CancellationToken c)
        {
            if (resolvedEvent.Event.EventType.StartsWith("$"))
                return;

            var eventJson = Encoding.UTF8.GetString(resolvedEvent.Event.Data.ToArray());
            var eventType = Type.GetType($"EventSourced.Simple.Aggregate.Events.{resolvedEvent.Event.EventType}, EventSourced.Simple");
            var @event = JsonConvert.DeserializeObject(eventJson, eventType);

            await projection.Handle(@event);
        }

        private async void OnSubscriptionDropped(StreamSubscription _, SubscriptionDroppedReason reason, Exception c)
        {
            await Subscribe();
        }

        private bool IsAllStream(string streamName)
        {
            return streamName.Equals("$all", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}