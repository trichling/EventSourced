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
        private readonly ITypeResovler typeResovler;

        public EventSourcedContext(IStreamStore streamStore, ITypeResovler typeResovler)
            :this(streamStore, new EventStream(), typeResovler, new AllPersistenceIdsProjection(streamStore))
        {
        }

        public EventSourcedContext(IStreamStore streamStore, IEventStream eventStream, ITypeResovler typeResovler, IAllPersistenceIdsProjection allPersistenceIdsProjection)
        {
            this.streamStore = streamStore;
            EventStream = eventStream;
            this.typeResovler = typeResovler;
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
                var eventType = typeResovler.ResolveFrom(message.Type);
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
