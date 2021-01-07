using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SqlStreamStore;
using SqlStreamStore.Streams;

namespace EventSourced.Simple.Framework
{

    public class SqlStreamStoreRepository<T> : IRepository<T> where T : EventSourcedBase
    {
        private readonly IStreamStore streamStore;

        public SqlStreamStoreRepository(IStreamStore streamStore)
        {
            this.streamStore = streamStore;
        }

        public async Task<T> Get(Func<T> factory)
        {
            var instance = factory();

            var streamId = new StreamId(instance.PersistenceId);
            var readStreamPage = await streamStore.ReadStreamForwards(streamId, StreamVersion.Start, int.MaxValue);

            foreach (var message in readStreamPage.Messages)
            {
                var eventJson = await message.GetJsonData();
                var eventType = Type.GetType($"EventSourced.Simple.Aggregate.Events.{message.Type}, EventSourced.Simple");
                var @event = JsonConvert.DeserializeObject(eventJson, eventType);
                instance.OnRecover(@event);
            }

            return (T)instance;
        }

        public async Task Save(T aggregate)
        {
            var uncomittedEvents = aggregate.UncomittedEvents;
            var streamId = new StreamId(aggregate.PersistenceId);

            foreach (var @event in uncomittedEvents)
            {
                var message = new NewStreamMessage(Guid.NewGuid(), @event.GetType().Name, JsonConvert.SerializeObject(@event));
                var appendResult = await streamStore.AppendToStream(streamId, ExpectedVersion.Any, message);
            }

            aggregate.Commit();
        }
    }
}