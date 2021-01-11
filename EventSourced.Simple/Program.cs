using System;
using System.Net.Http;
using System.Threading.Tasks;
using EventSourced.Simple.Aggregate;
using EventSourced.Simple.Framework;
using EventSourced.Simple.ReadModel;
using EventStore.Client;
using SqlStreamStore;

namespace EventSourced.Simple
{
    public class Program
    {

        static IRepository<Counter> repository;

        static async Task Main(string[] args)
        {
            await WithEventStore();
        }

        public static async Task WithEventStore()
        {
            var connectionString = "esdb://localhost:2113?tls=true&tlsVerifyCert=false";
            var settings = EventStoreClientSettings.Create(connectionString);
            settings.DefaultCredentials = new UserCredentials("admin", "changeit");
            var esClient = new EventStoreClient(settings);
            var esPersistenSubscriptionClient = new EventStorePersistentSubscriptionsClient(settings);

            var projection = await CreateProjection(settings, persistent: false);

            Console.ReadLine();

            repository = new EventStoreRepository<Counter>(esClient);

            for (int i = 0; i < 10; i++)
            {
                var counterId = Guid.NewGuid(); // Guid.Parse("fbb0f16b-646a-45d3-a1ee-596217897b61");
                await CreateAndSaveCounter(counterId);
                await LoadAndUpdateCounter(counterId);
            }

            Console.ReadLine();

            projection.Unsubscribe();
        }

        private static async Task<IProjectionHost> CreateProjection(EventStoreClientSettings settings, bool persistent)
        {
            IProjectionHost projectionHost;
            var currentCounterValuesReadModelBuilder = new CurrentCounterValuesReadModelBuilder();
           
            if (persistent)
            {
                var esPersistenSubscriptionClient = new EventStorePersistentSubscriptionsClient(settings);
                projectionHost = new EsPersistentSubscriptionProjectionHost(currentCounterValuesReadModelBuilder, esPersistenSubscriptionClient);
            }
            else
            {
                var esClient = new EventStoreClient(settings);
                projectionHost = new EsVolatileSubscriptionProjecctionHost(currentCounterValuesReadModelBuilder, esClient, new NoCheckpoinProvider());
            }

            await projectionHost.Subscribe();
            return projectionHost;
        }

        public static async Task WithSqlStreamStore()
        {
            var connectionString = "Server=(local);Database=SqlStreamStoreDemo;Trusted_Connection=True;MultipleActiveResultSets=true";
            var settings = new MsSqlStreamStoreV3Settings(connectionString);
            var streamStore = new MsSqlStreamStoreV3(settings);
            await streamStore.CreateSchemaIfNotExists();

            repository = new SqlStreamStoreRepository<Counter>(streamStore);

            var counterId = Guid.Parse("fbb0f16b-646a-45d3-a1ee-596217897b62");
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
