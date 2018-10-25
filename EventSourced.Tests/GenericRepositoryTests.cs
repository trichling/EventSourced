using System;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using EventSourced.Example.Aggregate;
using EventSourced.Example.Aggregate.Commands;
using EventSourced.Framework;
using Newtonsoft.Json;
using SqlStreamStore;
using SqlStreamStore.Streams;
using Xunit;

namespace EventSourced.Tests
{

    public interface IRepository<T> where T : EventSourcedBase
    {
        T Get(Guid id, Expression<Func<T>> factory);
        void Save(T entity);
    }

    public class GenericRepository<T> : IRepository<T> where T : EventSourcedBase
    {
        private readonly IStreamStore streamStore;
        private readonly EventSourcedInMemoryContext context;

        public GenericRepository(IStreamStore streamStore)
        {
            this.streamStore = streamStore;
            this.context = new EventSourcedInMemoryContext();
        }

        public EventSourcedInMemoryContext Context => context;

        public T Get(Guid id, Expression<Func<T>> factory)
        {
            var instance = context.Get(factory).GetAwaiter().GetResult();
            var streamId = new StreamId(instance.PersistenceId);
            var readStreamPage = streamStore.ReadStreamForwards(streamId, StreamVersion.Start, int.MaxValue).GetAwaiter().GetResult();

            foreach (var message in readStreamPage.Messages)
            {
                var eventJson = message.GetJsonData().GetAwaiter().GetResult();
                var eventType = Type.GetType($"EventSourced.Example.Aggregate.Events.{message.Type}, EventSourced.Example");
                var @event = JsonConvert.DeserializeObject(eventJson, eventType);
                instance.OnRecover(@event);
            }

            return (T)instance;
        }

        public void Save(T entity)
        {
            var uncomittedEvents = context.GetUncommitedEvents(entity.PersistenceId);
            var streamId = new StreamId(entity.PersistenceId);

            foreach (var @event in uncomittedEvents)
            {
                var message = new NewStreamMessage(Guid.NewGuid(), @event.GetType().Name, JsonConvert.SerializeObject(@event));
                var appendResult = streamStore.AppendToStream(streamId, ExpectedVersion.Any, message).GetAwaiter().GetResult();
            }

            context.Commit(entity.PersistenceId);
        }
    }

    public class GenericRepositoryTests
    {

         [Fact]
        public async Task CanSaveAndGetACounter()
        {
            IStreamStore streamStore = new InMemoryStreamStore();
            var repository = new GenericRepository<Counter>(streamStore);

            var counterId = Guid.NewGuid();
            var counter = repository.Get(counterId, () => new Counter(counterId));

            counter.Handle(new InitializeCounter(5));
            counter.Handle(new IncrementCounter(2));
            counter.Handle(new DecrementCounter(1));

            Assert.Equal(3, repository.Context.GetUncommitedEvents(counter.PersistenceId).Count());

            repository.Save(counter);

            Assert.Equal(0, repository.Context.GetUncommitedEvents(counter.PersistenceId).Count());
            Assert.Equal(2, streamStore.ReadHeadPosition().Result);

            var counterReloaded = repository.Get(counterId, () => new Counter(counterId));
            
            Assert.NotNull(counterReloaded);
        }

        [Fact]
        public async Task CanCreateUnitnializedObject()
        {
            var someClass = (SomeClass)FormatterServices.GetUninitializedObject(typeof(SomeClass));

            Assert.NotNull(someClass);
            Assert.IsType<SomeClass>(someClass);
        }

        private class SomeClass
        {
            
            public SomeClass(string aString, int anInt)
            {
                AString = aString;
                AnInt = anInt;
            }

            public string AString { get; }
            public int AnInt { get; }
        }

    }

}