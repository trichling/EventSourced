using System;
using System.Threading.Tasks;

namespace EventSourced.Framework.Abstractions
{

    public class ReadModelBase : IReadModel
    {
        private readonly IEventSourcingSystem system;
        protected bool hasCaughtUp;
        protected long lastPosition;
        protected IDisposable subscription;

        public ReadModelBase(IEventSourcingSystem system)
        {
            this.system = system;
            this.lastPosition = -1;
        }

        public long LastPosition => lastPosition;

        public bool IsUpToDate => hasCaughtUp && lastPosition == system.EventStore.StoreVersion();

        public void StartCatchingUpFrom(long lastPosition)
        {
            subscription = this.system.EventStore.CatchUpSubscription(lastPosition, OnEvent, OnHasCaughtUp);
        }

        public async Task WaitForCatchUp()
        {
            while (!IsUpToDate)
                await Task.Delay(10);

            subscription.Dispose();
        }

        public virtual void Apply(string persistenceId, long? position, object @event)
        {
            if (position.HasValue)
               lastPosition = position.Value;

            ((dynamic)this).Apply((dynamic)@event);
        }

        protected virtual void OnEvent(string persistenceId, long? position, object @event)
        {
            if (position.HasValue)
                lastPosition = position.Value;

            if (!IsUpToDate)
                ((dynamic)this).Apply((dynamic)@event);
        }

        protected virtual void OnHasCaughtUp()
        {
            hasCaughtUp = true;
        }
    }
}