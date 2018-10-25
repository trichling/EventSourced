using System;
using Xunit;
using SqlStreamStore;
using SqlStreamStore.Streams;
using System.Threading.Tasks;
using System.Threading;
using SqlStreamStore.Subscriptions;
using System.Runtime.Serialization;

namespace EventSourced.Tests
{
    public class SqlStreamStoreAPITests
    {
       
        [Fact]
        public async Task CanAppendToStreamAndReadFromStreamForwardAndGetJsonBack()
        {
            var streamStore = new InMemoryStreamStore();
            var stremId = new StreamId("test");

            var jsonIn =  @"{ 'Hello': 'World' }";
            var message = new NewStreamMessage(Guid.NewGuid(), "Test", jsonIn);
            await streamStore.AppendToStream(stremId, ExpectedVersion.Any, message);

            var readStreamPage = await streamStore.ReadStreamForwards(stremId, StreamVersion.Start, int.MaxValue);
            var jsonOut = await readStreamPage.Messages[0].GetJsonData();

            Assert.Equal(1, readStreamPage.Messages.Length);
            Assert.Equal(jsonIn, jsonOut);
        }

        [Fact]
        public async Task AppendToStreamWithWrongExpectedVersionThrowsException()
        {
            var streamStore = new InMemoryStreamStore();
            var stremId = new StreamId("test");

            var jsonIn =  @"{ 'Hello': 'World' }";
            var message = new NewStreamMessage(Guid.NewGuid(), "Test", jsonIn);
            await streamStore.AppendToStream(stremId, ExpectedVersion.NoStream, message);

            jsonIn = @"{ 'Hello': 'New World' }";
            message = new NewStreamMessage(Guid.NewGuid(), "Test", jsonIn);
            await streamStore.AppendToStream(stremId, 0, message);

            jsonIn = @"{ 'Hello': 'Very New World' }";
            message = new NewStreamMessage(Guid.NewGuid(), "Test", jsonIn);
            await Assert.ThrowsAsync(typeof(WrongExpectedVersionException), async () => {await streamStore.AppendToStream(stremId, 0, message); });
        }

        [Fact]
        public async Task CanAppendToMultipleStreamsAndReadAllForward()
        {
            var streamStore = new InMemoryStreamStore();
            var stream1 = new StreamId("test1");
            var stream2 = new StreamId("test2");

            var message1 = new NewStreamMessage(Guid.NewGuid(), "Test1", @"{ 'Hello': 'World1' }");
            await streamStore.AppendToStream(stream1, ExpectedVersion.Any, message1);

            var message2 = new NewStreamMessage(Guid.NewGuid(), "Test2", @"{ 'Hello': 'World2' }");
            await streamStore.AppendToStream(stream2, ExpectedVersion.Any, message2);

            var readStreamPage = await streamStore.ReadAllForwards(StreamVersion.Start, int.MaxValue, true);

            Assert.Equal(2, readStreamPage.Messages.Length);
        }

        [Fact]
        public async Task CanSubscribeToStream()
        {
            var streamMessageReceived = false;
            var subscriptionDropped = false;

            var streamStore = new InMemoryStreamStore();
            var streamId = new StreamId("test");

            streamStore.SubscribeToStream(streamId, null, StreamMessageReceived, SubscriptionDropped);

            var message = new NewStreamMessage(Guid.NewGuid(), "Test", @"{ 'Hello': 'World' }");
            await streamStore.AppendToStream(streamId, ExpectedVersion.Any, message);

            Thread.Sleep(100);

            Assert.True(streamMessageReceived);
            Assert.False(subscriptionDropped);

            async Task StreamMessageReceived(IStreamSubscription subscription, StreamMessage streamMessage, CancellationToken cancellationToken)
            {
                streamMessageReceived = true;
                Assert.Equal("Test", streamMessage.Type);
            }

            void SubscriptionDropped(IStreamSubscription subscription, SubscriptionDroppedReason reason, Exception exception = null)
            {
                subscriptionDropped = true;
            }
        }

         [Fact]
        public async Task CanSubscribeToAllStreams()
        {
            var receivedMessagesCount = 0;
            var subscriptionDropped = false;

            var streamStore = new InMemoryStreamStore();
            var stream1 = new StreamId("test1");
            var stream2 = new StreamId("test2");

            streamStore.SubscribeToAll(null, AllStreamMessageReceived, AllSubscriptionDropped);

            var message1 = new NewStreamMessage(Guid.NewGuid(), "Test1", @"{ 'Hello': 'World1' }");
            await streamStore.AppendToStream(stream1, ExpectedVersion.Any, message1);

            var message2 = new NewStreamMessage(Guid.NewGuid(), "Test2", @"{ 'Hello': 'World2' }");
            await streamStore.AppendToStream(stream2, ExpectedVersion.Any, message2);

            Thread.Sleep(100);

            Assert.Equal(2, receivedMessagesCount);
            Assert.False(subscriptionDropped);

            async Task AllStreamMessageReceived(IAllStreamSubscription subscription, StreamMessage streamMessage, CancellationToken cancellationToken)
            {
                if (receivedMessagesCount == 0)
                    Assert.Equal("Test1", streamMessage.Type);

                if (receivedMessagesCount == 1)
                    Assert.Equal("Test2", streamMessage.Type);

                receivedMessagesCount++;
            }

            void AllSubscriptionDropped(IAllStreamSubscription subscription, SubscriptionDroppedReason reason, Exception exception = null)
            {
                subscriptionDropped = true;
            }
        }

      
    }
}
