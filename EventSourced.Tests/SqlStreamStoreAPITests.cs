using System;
using Xunit;
using SqlStreamStore;
using SqlStreamStore.Streams;
using System.Threading.Tasks;

namespace EventSourced.Tests
{
    public class SqlStreamStoreAPITests
    {
        [Fact]
        public async Task CanAppendToStream()
        {
            var streamStore = new InMemoryStreamStore();

            var message = new NewStreamMessage(Guid.NewGuid(), "Test", @"{ 'Hello': 'World' }");
            streamStore.AppendToStream(new StreamId("test"), ExpectedVersion.Any, message);

            var readStreamPage = await streamStore.ReadStreamForwards(new StreamId("test"), StreamVersion.Start, int.MaxValue);

            Assert.Equal(1, readStreamPage.Messages.Length);
        }
    }
}
