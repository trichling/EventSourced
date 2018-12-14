using System;
using System.Threading.Tasks;
using EventSourced.Simple.Aggregate;
using EventSourced.Simple.Framework;
using SqlStreamStore;

namespace EventSourced.Simple
{
    public class Program
    {

        static IStreamStore streamStore;
        static IRepository<Counter> repository;

        static async Task Main(string[] args)
        {
            var connectionString = "Server=(local);Database=SqlStreamStoreDemo;Trusted_Connection=True;MultipleActiveResultSets=true";
            var settings = new MsSqlStreamStoreSettings(connectionString);
            var streamStore = new MsSqlStreamStore(settings);
            await streamStore.CreateSchema();

            repository = new Repository<Counter>(streamStore);

            var counterId = Guid.Parse("fbb0f16b-646a-45d3-a1ee-596217897b63");
            await CreateAndSaveCounter(counterId);
            await LoadAndUpdateCounter(counterId);
        }

        public static async Task CreateAndSaveCounter(Guid counterId)
        {
            var counter = await repository.Get(() => new Counter(counterId));

            counter.Initialize(0);
            counter.Increment(5);
            counter.Decrement(2);
           
            await repository.Save(counter);
        }

        public static async Task LoadAndUpdateCounter(Guid counterId)
        {
            var counter = await repository.Get(() => new Counter(counterId));

            counter.Increment(3);
            counter.Decrement(2);

            await repository.Save(counter);
        }
    }
}
