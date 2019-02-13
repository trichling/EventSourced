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
    public class EventSourcingSystem : IEventSourcingSystem
    {

        private IEventSourcingSystem underlyingSystem;

        public EventSourcingSystem(IEventSourcingSystem underlyingSystem)
        {
            this.underlyingSystem = underlyingSystem;
        }

        public EventSourcingSystem(IEventStore eventStore)
        {
            this.underlyingSystem = new EventSourcingSystemWithImmediateCommit(eventStore, new EventStream());
        }

        public EventSourcingSystem(IEventStore eventStore, IEventStream eventStream)
        {
            this.underlyingSystem = new EventSourcingSystemWithImmediateCommit(eventStore, eventStream);
        }

        public EventSourcingSystem WithImmediateCommit()
        {
            this.underlyingSystem = new EventSourcingSystemWithImmediateCommit(this.EventStore, this.EventStream);
            return this;
        }

        public EventSourcingSystem WithExplicitCommit()
        {
            this.underlyingSystem = new EventSourcingSystemWithExplicitCommit(this.EventStore, this.EventStream);
            return this;
        }

        public IEventStore EventStore => this.underlyingSystem.EventStore;
        public IEventStream EventStream => this.underlyingSystem.EventStream;

        
        public Task<T> Get<T>(Func<T> factory) where T : EventSourcedBase
        {
            return this.underlyingSystem.Get<T>(factory);
        }

        public Task<bool> Save(string persistenceId, dynamic @event)
        {
            return this.underlyingSystem.Save(persistenceId, @event);
        }

        public Task<bool> Commit(string persistenceId)
        {
            return this.underlyingSystem.Commit(persistenceId);
        }

       
    }
}
