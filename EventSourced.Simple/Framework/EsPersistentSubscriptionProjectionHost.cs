using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EventStore.Client;
using Newtonsoft.Json;

namespace EventSourced.Simple.Framework
{
    public class EsPersistentSubscriptionProjectionHost : IProjectionHost
    {
        private readonly IProjection projection;
        private readonly EventStorePersistentSubscriptionsClient client;
        private PersistentSubscription subscription;

        public EsPersistentSubscriptionProjectionHost(IProjection projection, EventStorePersistentSubscriptionsClient client)
        {
            this.projection = projection;
            this.client = client;
        }

        public async Task Subscribe()
        {
            await EnsureSubscription();
            subscription = await client.SubscribeAsync(projection.Stream, projection.Name, OnEvent, OnSubscriptionDropped);       
        }

        private async Task EnsureSubscription()
        {
            var settings = new PersistentSubscriptionSettings(true, StreamPosition.Start);
            try
            {
                await client.CreateAsync(projection.Stream, projection.Name, settings);
            }   
            catch (Exception) 
            {
                // Subscription already exists
            }        
        }

        private async Task OnEvent(PersistentSubscription _, ResolvedEvent resolvedEvent, int? arg3, CancellationToken token)
        {
            if (resolvedEvent.Event.EventType.StartsWith("$"))
                return;

            var eventJson = Encoding.UTF8.GetString(resolvedEvent.Event.Data.ToArray());
            var eventType = Type.GetType($"EventSourced.Simple.Aggregate.Events.{resolvedEvent.Event.EventType}, EventSourced.Simple");
            var @event = JsonConvert.DeserializeObject(eventJson, eventType);

            await projection.Handle(@event);
        }

        private async void OnSubscriptionDropped(PersistentSubscription _, SubscriptionDroppedReason arg2, Exception arg3)
        {
            await Subscribe();
        }

        public void Unsubscribe()
        {
            subscription.Dispose();
        }
    }
}   