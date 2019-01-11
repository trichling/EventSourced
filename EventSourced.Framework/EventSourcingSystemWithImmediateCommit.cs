using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using EventSourced.Framework.Abstracions;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using SqlStreamStore;
using SqlStreamStore.Streams;

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

        public Task<bool> Save(string persistenceId, dynamic @event)
        {
            return EventStore.Persist(persistenceId, @event);
        }

        public Task<bool> Commit(string persistenceId)
        {
            return Task.FromResult(true);
        }

       
    }
}
