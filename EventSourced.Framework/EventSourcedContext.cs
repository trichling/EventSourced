using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using SqlStreamStore;
using SqlStreamStore.Streams;

namespace EventSourced.Framework
{
    public class EventSourcedContext
    {
        private IStreamStore streamStore;

        public EventSourcedContext(IStreamStore streamStore)
            :this(streamStore, new EventStream(), new AllPersistenceIdsProjection(streamStore))
        {
        }

        public EventSourcedContext(IStreamStore streamStore, IEventStream eventStream, IAllPersistenceIdsProjection allPersistenceIdsProjection)
        {
            this.streamStore = streamStore;
            EventStream = eventStream;
            AllStreams = allPersistenceIdsProjection;
        }

        public IEventStream EventStream { get; }
        public IAllPersistenceIdsProjection AllStreams { get; }

        public async Task<T> Get<T>(Expression<Func<T>> factory, params object[] args) where T : EventSourcedBase
        {
            var instance = (T)factory.Compile().DynamicInvoke(args);
            instance.Context = this;

            var streamId = new StreamId(instance.PersistenceId);
            var readStreamPage = await streamStore.ReadStreamForwards(streamId, StreamVersion.Start, int.MaxValue);

            foreach (var message in readStreamPage.Messages)
            {
                var eventJson = await message.GetJsonData();
                var eventTypeName = $"EventSourced.Example.Aggregate.Events.{message.Type}, EventSourced.Example";
                var eventType = Type.GetType(eventTypeName);
                var @event = JsonConvert.DeserializeObject(eventJson, eventType);
                instance.OnRecover(@event);
            }

            return instance;
        }
        
        internal async Task<bool> Persist(string persistenceId, object @event)
        {
            var streamId = new StreamId(persistenceId);
            var message = new NewStreamMessage(Guid.NewGuid(), @event.GetType().Name, JsonConvert.SerializeObject(@event));
            var appendResult = await streamStore.AppendToStream(streamId, ExpectedVersion.Any, message);

            return true;
        }
    }
}
