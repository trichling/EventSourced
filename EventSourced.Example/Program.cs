using System;
using System.Threading;
using System.Threading.Tasks;
using Lab.SqlStreamStoreDemo.ExampleAggregate;
using Lab.SqlStreamStoreDemo.ExampleAggregate.Commands;
using Lab.SqlStreamStoreDemo.ExampleReadModel;
using Lab.SqlStreamStoreDemo.Framework;
using Microsoft.Extensions.DependencyInjection;
using SqlStreamStore;

namespace Lab.SqlStreamStoreDemo
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var connectionString = "Server=(local);Database=SqlStreamStoreDemo;Trusted_Connection=True;MultipleActiveResultSets=true";
            var settings = new MsSqlStreamStoreSettings(connectionString);
            var streamStore = new MsSqlStreamStore(settings);
            streamStore.CreateSchema().GetAwaiter().GetResult();

            //var domainContext = new EventSourcedContext(streamStore, new SqlStreamStoreEventStream(streamStore));
            var domainContext = new EventSourcedContext(streamStore);

            // var readModel = new CounterCurrentValuesReadModelBuilder(domainContext);

            var counterId = Guid.Parse("8c936406-720a-45d4-b1e0-a95bd595943f");
            var counter = await domainContext.Get<Counter>(() => new Counter(counterId));
            // counter.Handle(new InitializeCounter(5));
            // counter.Handle(new IncrementCounter(8));
            // counter.Handle(new DecrementCounter(3));

            Thread.Sleep(100);
        }

        private static async Task ACounter()
        {
            // var domainContext = new DomainContext("Server=(local);Database=SqlStreamStoreDemo;Trusted_Connection=True;MultipleActiveResultSets=true");
            // var counterId = Guid.Parse("8c936406-720a-45d4-b1e0-a95bd595943e");
            // var counter = await domainContext.Get<Counter>(() => new Counter(counterId));
            
            // counter.Handle(new InitializeCounter(5));
            // counter.Handle(new IncrementCounter(8));
            // counter.Handle(new DecrementCounter(3));
        }
    }
}
