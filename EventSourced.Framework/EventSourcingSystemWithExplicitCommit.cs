using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using EventSourced.Framework.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using SqlStreamStore;
using SqlStreamStore.Streams;

namespace EventSourced.Framework
{
    public class EventSourcingSystemWithExplicitCommit : IEventSourcingSystem
    {

        private Dictionary<string, List<object>> uncommitedEvents;

        public EventSourcingSystemWithExplicitCommit(IEventStore eventStore, IEventStream eventStream)
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

        public Task Save(string persistenceId, dynamic @event)
        {
            if (!uncommitedEvents.ContainsKey(persistenceId))
                uncommitedEvents.Add(persistenceId, new List<object>());

            uncommitedEvents[persistenceId].Add(@event);

            return Task.CompletedTask;
        }

        public async Task<bool> Commit(string persistenceId)
        {
            if (!uncommitedEvents.ContainsKey(persistenceId))
               return true;

            foreach (var @event in uncommitedEvents[persistenceId])
            {
                var newPosition = await EventStore.Persist(persistenceId, @event);
                EventStream.Publish(new Event(persistenceId, newPosition, @event));            
            }

            uncommitedEvents[persistenceId].Clear();

            return true;
        }
       
    }
}
