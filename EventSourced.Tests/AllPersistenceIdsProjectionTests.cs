using System;
using Xunit;
using SqlStreamStore;
using SqlStreamStore.Streams;
using System.Threading.Tasks;
using System.Threading;
using SqlStreamStore.Subscriptions;
using EventSourced.Framework;
using EventSourced.Framework.Abstracions;
using EventSourced.Framework.SqlStreamStore;

namespace EventSourced.Tests
{
    public class AllPersistenceIdsProjectionTests
    {
       
        [Fact]
        public async Task GivenExistingEventStore_CanBuildAllPersistenceIdsProjection()
        {
            var streamStore = new InMemoryStreamStore();
            var tpyeResovler = new FullyQualifiedTypeNameTypeResolver("EventSourced.Example.Aggregate.Events.{0}, EventSourced.Example");
            var eventStore = new SqlStreamStoreEventStore(streamStore, tpyeResovler);

            var system = new EventSourcingSystem(eventStore);

            await system.EventStore.Persist("test1", new { Data = "Test1" });
            await system.EventStore.Persist("test2", new { Data = "Test2" });

            var allPersistenceIdsProjection = new AllPresistenceIdsReadModel(system);

            Assert.Equal(2, allPersistenceIdsProjection.PersistenceIds.Count);
        }

        [Fact]
        public async Task GivenExistingEventStore_WhenAddingAnEventToTheStore_AllPersistenceIdsProjectionIsUpdated()
        {
            var streamStore = new InMemoryStreamStore();
            var tpyeResovler = new FullyQualifiedTypeNameTypeResolver("EventSourced.Example.Aggregate.Events.{0}, EventSourced.Example");
            var eventStore = new SqlStreamStoreEventStore(streamStore, tpyeResovler);

            var system = new EventSourcingSystem(eventStore);

            await system.EventStore.Persist("test1", new { Data = "Test1" });
            await system.EventStore.Persist("test2", new { Data = "Test2" });

            var allPersistenceIdsProjection = new AllPresistenceIdsReadModel(system);

            Assert.Equal(2, allPersistenceIdsProjection.PersistenceIds.Count);

            await system.EventStore.Persist("test3", new { Data = "Test3" });

            await Task.Delay(10);

            Assert.Equal(3, allPersistenceIdsProjection.PersistenceIds.Count);
        }
    }
}
