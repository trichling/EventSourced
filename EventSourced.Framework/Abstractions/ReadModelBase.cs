using System.Threading.Tasks;
using EventSourced.Framework.Abstracions;

namespace EventSourced.Framework.Abstracions
{

    public class ReadModelBase : IReadModel
    {
        private readonly IEventSourcingSystem system;
        private bool hasCaughtUp;
        private long lastPosition;

        public ReadModelBase(IEventSourcingSystem system)
        {
            this.system = system;
        }

        public virtual bool IsUpToDate => system.EventStore.StoreVersion().Result == -1 || (hasCaughtUp && lastPosition == system.EventStore.StoreVersion().Result);

        public async Task CatchUp()
        {
            this.system.EventStore.CatchUpSubscription(OnEvent, OnHasCaughtUp);
            await WaitForCatchUp();
        }

        private async Task WaitForCatchUp()
        {
            while (!IsUpToDate)
                await Task.Delay(10);

            // somehow cancel the subscription!!
        }

        public virtual void Handle(object @event)
        {
        }

        protected virtual void OnEvent(string persistenceId, dynamic @event, long position)
        {
            lastPosition = position;

            if (!IsUpToDate)
                ((dynamic)this).Handle(@event);
        }

        protected virtual void OnHasCaughtUp()
        {
            hasCaughtUp = true;
        }
    }
}