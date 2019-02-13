using System;
using System.Threading.Tasks;
using EventSourced.Framework.Abstracions;

namespace EventSourced.Framework
{
    public class EventSourcingSystemWithImmediateCommit : IEventSourcingSystem
    {

        public EventSourcingSystemWithImmediateCommit(IEventStore eventStore, IEventStream eventStream)
        {
            EventStore = eventStore;
            EventStream = eventStream;
        }

        public IEventStore EventStore { get; }
        public IEventStream EventStream { get; }

        
        public async Task<T> Get<T>(Func<T> factory) where T : EventSourcedBase
        {
            var instance = factory();
            var history = await EventStore.GetHistory(instance.PersistenceId);

            instance.Recover(history);
            instance.Context = this;

            return instance;
        }

        public async Task<bool> Save(string persistenceId, dynamic @event)
        {
            var persistSuccessful = await EventStore.Persist(persistenceId, @event);
            if (persistSuccessful)
                EventStream.Publish(@event);
                
            return persistSuccessful;
        }

        public Task<bool> Commit(string persistenceId)
        {
            return Task.FromResult(true);
        }

       
    }
}
