using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace EventSourced.Framework
{
    public class EventSourcedInMemoryContext : IEventSourcedContext
    {
        private IEventStream eventStream;
        private IAllPersistenceIdsProjection allPersistenceIdsProjection;

        private Dictionary<string, List<object>> uncommitedEvents;

        public EventSourcedInMemoryContext()
        {
            this.uncommitedEvents = new Dictionary<string, List<object>>();
            this.eventStream = new EventStream();
            this.allPersistenceIdsProjection = new AllPersistenceIdsInMemoryProjection(uncommitedEvents.Keys);
        }

        public EventSourcedInMemoryContext(EventStream eventStream, IAllPersistenceIdsProjection allPersistenceIdsProjection)
        {
            this.eventStream = eventStream;
            this.allPersistenceIdsProjection = allPersistenceIdsProjection;
            this.uncommitedEvents = new Dictionary<string, List<object>>();
        }

        public IEventStream EventStream => eventStream;

        public IAllPersistenceIdsProjection AllStreams => allPersistenceIdsProjection;

        public Task<T> Get<T>(Expression<Func<T>> factory, params object[] args) where T : EventSourcedBase
        {
            var instance = (T)factory.Compile().DynamicInvoke();
            instance.Context = this;

            return Task.FromResult(instance);
        }

        public Task<bool> Persist(string persistenceId, object @event)
        {
            if (!uncommitedEvents.ContainsKey(persistenceId))
                uncommitedEvents.Add(persistenceId, new List<object>());

            uncommitedEvents[persistenceId].Add(@event);

            return Task.FromResult(true);
        }

        public IEnumerable<object> GetUncommitedEvents(string persistenceId)
        {
            if (!uncommitedEvents.ContainsKey(persistenceId))
                uncommitedEvents.Add(persistenceId, new List<object>());

            return uncommitedEvents[persistenceId];
        }

        public void Commit(string persistenceId)
        {
            if (!uncommitedEvents.ContainsKey(persistenceId))
                uncommitedEvents.Add(persistenceId, new List<object>());

            uncommitedEvents[persistenceId].Clear();
        }
    }
}