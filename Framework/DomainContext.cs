using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using SqlStreamStore;
using SqlStreamStore.Streams;

namespace Lab.SqlStreamStoreDemo.Framework
{
    public class DomainContext
    {
        private ServiceCollection serviceCollection;
        private ServiceProvider serviceProvider;

        public DomainContext(string connectionString)
        {
            var settings = new MsSqlStreamStoreSettings(connectionString);
            var streamStore = new MsSqlStreamStore(settings);
            streamStore.CreateSchema().GetAwaiter().GetResult();

            serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<EventHandlerRepository>(new EventHandlerRepository());
            serviceCollection.AddTransient<IStreamStore, MsSqlStreamStore>(p => new MsSqlStreamStore(settings));
            serviceCollection.AddMediatR();
            serviceProvider =  serviceCollection.BuildServiceProvider();

            
            StreamStore = serviceProvider.GetService<IStreamStore>();
            Mediator = serviceProvider.GetService<IMediator>();
            EventStream = serviceProvider.GetService<EventHandlerRepository>();
        }

        public IMediator Mediator { get; }
        public EventHandlerRepository EventStream { get; }
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
