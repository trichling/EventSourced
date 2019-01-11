using System;
using Xunit;
using SqlStreamStore;
using SqlStreamStore.Streams;
using System.Threading.Tasks;
using System.Threading;
using SqlStreamStore.Subscriptions;
using EventSourced.Framework;
using EventSourced.Framework.SqlStreamStore;

namespace EventSourced.Tests
{
    public class AllPersistenceIdsProjectionTests
    {
       
        [Fact]
        public async Task GivenExistingEventStore_CanBuildAllPersistenceIdsProjection()
        {
            var streamStore = new InMemoryStreamStore();

            var stream1 = new StreamId("test1");
            var message1 = new NewStreamMessage(Guid.NewGuid(), "Test1", @"{ 'Hello': 'World1' }");
            await streamStore.AppendToStream(stream1, ExpectedVersion.Any, message1);

            var stream2 = new StreamId("test2");
            var message2 = new NewStreamMessage(Guid.NewGuid(), "Test2", @"{ 'Hello': 'World2' }");
            await streamStore.AppendToStream(stream2, ExpectedVersion.Any, message2);

            var allPersistenceIdsProjection = new AllPersistenceIdsSqlStreamStoreProjection(streamStore);

            await allPersistenceIdsProjection.WaitUntilIsUpToDate();

            Assert.Equal(2, allPersistenceIdsProjection.StreamIds.Count);
        }

        [Fact]
        public async Task GivenExistingEventStore_WhenAddingAnEventToTheStore_AllPersistenceIdsProjectionIsUpdated()
        {
            var streamStore = new InMemoryStreamStore();

             var stream1 = new StreamId("test1");
            var message1 = new NewStreamMessage(Guid.NewGuid(), "Test1", @"{ 'Hello': 'World1' }");
            await streamStore.AppendToStream(stream1, ExpectedVersion.Any, message1);

            var allPersistenceIdsProjection = new AllPersistenceIdsSqlStreamStoreProjection(streamStore);

            await allPersistenceIdsProjection.WaitUntilIsUpToDate();
            Assert.Equal(1, allPersistenceIdsProjection.StreamIds.Count);

            var stream2 = new StreamId("test2");
            var message2 = new NewStreamMessage(Guid.NewGuid(), "Test2", @"{ 'Hello': 'World2' }");
            await streamStore.AppendToStream(stream2, ExpectedVersion.Any, message2);

            await allPersistenceIdsProjection.WaitUntilIsUpToDate();
            Assert.Equal(2, allPersistenceIdsProjection.StreamIds.Count);
        }
    }
}
