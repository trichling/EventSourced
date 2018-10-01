using System;
using System.Threading.Tasks;
using Lab.SqlStreamStoreDemo.ExampleAggregate;
using Lab.SqlStreamStoreDemo.Framework;
using MediatR;
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
            var store = new MsSqlStreamStore(settings);
            await store.CreateSchema();

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddTransient<IStreamStore, MsSqlStreamStore>(p => new MsSqlStreamStore(settings));
            serviceCollection.AddTransient<DomainContext>();
            serviceCollection.AddMediatR();
            var serviceProvider =  serviceCollection.BuildServiceProvider();

            var domainContext = serviceProvider.GetService<DomainContext>();

            var counterId = Guid.Parse("8c936406-720a-45d4-b1e0-a95bd595943e");
            var counter = await domainContext.Get<Counter>(() => new Counter(counterId));
            // counter.Handle(new InitializeCounter(5));
            // counter.Handle(new IncrementCounter(8));
            // counter.Handle(new DecrementCounter(3));
        }
    }
}
