using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MediatR;
using Newtonsoft.Json;
using SqlStreamStore;
using SqlStreamStore.Streams;

namespace Lab.SqlStreamStoreDemo.Framework
{
    public class DomainContext
    {

        public DomainContext(IMediator mediator, IStreamStore streamStore)
        {
            Mediator = mediator;
            StreamStore = streamStore;
        }

        public IMediator Mediator { get; }
        public IStreamStore StreamStore { get; }

        public async Task<T> Get<T>(Expression<Func<T>> factory, params object[] args) where T : EventSourced
        {
            var instance = (T)factory.Compile().DynamicInvoke(args);
            instance.Context = this;

            var streamId = new StreamId(instance.PersistenceId);
            var readStreamPage = await StreamStore.ReadStreamForwards(streamId, StreamVersion.Start, int.MaxValue);

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
    }
}
