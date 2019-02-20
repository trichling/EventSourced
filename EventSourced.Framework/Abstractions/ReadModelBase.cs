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

        public virtual bool IsUpToDate => hasCaughtUp && lastPosition == system.EventStore.StoreVersion();

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

        public virtual void Handle(object @event)
        {
        }

        protected virtual void OnEvent(string persistenceId, long position, object @event)
        {
            lastPosition = position;

            if (!IsUpToDate)
                ((dynamic)this).Handle((dynamic)@event);
        }

        protected virtual void OnHasCaughtUp()
        {
            hasCaughtUp = true;
        }
    }
}