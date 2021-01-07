using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventStore.Client;
using Newtonsoft.Json;

namespace EventSourced.Simple.Framework
{
    public class EventStoreRepository<T> : IRepository<T> where T : EventSourcedBase
    {
        private readonly EventStoreClient eventStore;

        public EventStoreRepository(EventStoreClient eventStore)
        {
            this.eventStore = eventStore;
        }

        public async Task<T> Get(Func<T> factory)
        {
            var instance = factory();

            var stream = eventStore.ReadStreamAsync(Direction.Forwards, instance.PersistenceId, StreamPosition.Start);
            if (await stream.ReadState == ReadState.StreamNotFound)
                return instance;

            await foreach (var message in stream)
            {
                var eventJson = Encoding.UTF8.GetString(message.Event.Data.ToArray());
                var eventType = Type.GetType($"EventSourced.Simple.Aggregate.Events.{message.Event.EventType}, EventSourced.Simple");
                var @event = JsonConvert.DeserializeObject(eventJson, eventType);
                instance.OnRecover(@event);
            }

            return instance;
        }

        public async Task Save(T aggregate)
        {
            var uncomittedEvents = aggregate.UncomittedEvents;
            var events = uncomittedEvents.Select(e => {
                var eventJsonUtf8 = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(e));
                var message = new EventData(Uuid.NewUuid(), e.GetType().Name, eventJsonUtf8);
                return message;
            });

            await eventStore.AppendToStreamAsync(aggregate.PersistenceId, StreamState.Any, events);

            aggregate.Commit();
        }
    }
}