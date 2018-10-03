using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using SqlStreamStore;
using SqlStreamStore.Streams;

namespace Lab.SqlStreamStoreDemo.Framework
{
    public class DomainContext
    {
        private IStreamStore streamStore;

        public DomainContext(IStreamStore streamStore)
            :this(streamStore, new EventStream())
        {
        }

        public DomainContext(IStreamStore streamStore, IEventStream eventStream)
        {
            this.streamStore = streamStore;
            EventStream = eventStream;
        }

        public IEventStream EventStream { get; }

        public async Task<T> Get<T>(Expression<Func<T>> factory, params object[] args) where T : EventSourced
        {
            var instance = (T)factory.Compile().DynamicInvoke(args);
            instance.Context = this;

            var streamId = new StreamId(instance.PersistenceId);
            var readStreamPage = await streamStore.ReadStreamForwards(streamId, StreamVersion.Start, int.MaxValue);

            foreach (var message in readStreamPage.Messages)
            {
                var eventJson = await message.GetJsonData();
                var eventTypeName = $"Lab.SqlStreamStoreDemo.ExampleAggregate.Events.{message.Type}";
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
