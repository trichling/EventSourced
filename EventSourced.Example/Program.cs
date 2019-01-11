using System;
using System.Threading;
using System.Threading.Tasks;
using EventSourced.Framework;
using Microsoft.Extensions.DependencyInjection;
using SqlStreamStore;
using EventSourced.Example.Aggregate;
using EventSourced.Example.Example.ReadModel;
using EventSourced.Framework.SqlStreamStore;

namespace EventSourced.Example
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var tpyeResovler = new FullyQualifiedTypeNameTypeResolver("EventSourced.Example.Aggregate.Events.{0}, EventSourced.Example");
            var connectionString = "Server=(local);Database=SqlStreamStoreDemo;Trusted_Connection=True;MultipleActiveResultSets=true";
            var settings = new MsSqlStreamStoreSettings(connectionString);
            var streamStore = new MsSqlStreamStore(settings);
            streamStore.CreateSchema().GetAwaiter().GetResult();

            var eventStore = new SqlStreamStoreEventStore(streamStore, tpyeResovler);
            var system = new EventSourcingSystem(eventStore);

            var readModel = new CounterCurrentValuesReadModelBuilder(system);

            var counterId = Guid.Parse("8c936406-720a-45d4-b1e0-a95bd595943f");
            var counter = await system.Get(() => new Counter(counterId));

            counter.Initialize(0);
            counter.Increment(5);
            counter.Decrement(2);

            Thread.Sleep(5000);
        }

     
    }
}
