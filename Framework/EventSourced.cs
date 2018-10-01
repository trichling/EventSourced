using System;

namespace Lab.SqlStreamStoreDemo.Framework
{
    public abstract class EventSourced
    {

        internal DomainContext Context { get; set; }

        public abstract string PersistenceId { get; }

        public void OnCommand(object command)
        {
            ((dynamic)this).Handle((dynamic)command);
        }

        public void OnRecover(object @event)
        {
            DispatchToApply(@event);
        }

        public void Causes(object @event)
        {
            Persist(@event, Publish);
        }

        public async void Persist(object @event, Action<object> callback)
        {
            var persistSuccessful = Context.Mediator.Send(new PersistEvent(this.PersistenceId, @event)).GetAwaiter().GetResult();
            if (persistSuccessful)
                Publish(@event);
        }

        public void Publish(object @event)
        {
            Context.Mediator.Publish(new PublishedEvent(@event.GetType().Name,  @event));
            DispatchToApply(@event);
        }

         public void DispatchToApply(dynamic @event)
        {
            ((dynamic)this).Apply((dynamic)@event);
        }
    }
}
