using System;
using System.Net.Http;
using System.Threading.Tasks;
using EventSourced.Simple.Aggregate;
using EventSourced.Simple.Framework;
using EventStore.Client;
using SqlStreamStore;

namespace EventSourced.Simple
{
    public class Program
    {

        static IStreamStore streamStore;
        static IRepository<Counter> repository;

        static async Task Main(string[] args)
        {
            await WithEventStore();
        }

        public static async Task WithEventStore()
        {
            var connectionString = "esdb://localhost:2113?Tls=true";
            var settings = EventStoreClientSettings.Create(connectionString);
            settings.DefaultCredentials = new UserCredentials("admin", "changeit");
                        settings.CreateHttpMessageHandler = () => 
                            new HttpClientHandler 
                            {
                                ServerCertificateCustomValidationCallback =
                                    (sender, cert, chain, sslPolicyErrors) => true
                            };
            var eventStore = new EventStoreClient(settings);

            repository = new EventStoreRepository<Counter>(eventStore);

            var counterId = Guid.Parse("fbb0f16b-646a-45d3-a1ee-596217897b63");
            await CreateAndSaveCounter(counterId);
            await LoadAndUpdateCounter(counterId);
        }

        public static async Task WithSqlStreamStore()
        {
            var connectionString = "Server=(local);Database=SqlStreamStoreDemo;Trusted_Connection=True;MultipleActiveResultSets=true";
            var settings = new MsSqlStreamStoreV3Settings(connectionString);
            var streamStore = new MsSqlStreamStoreV3(settings);
            await streamStore.CreateSchemaIfNotExists();

            repository = new SqlStreamStoreRepository<Counter>(streamStore);

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
