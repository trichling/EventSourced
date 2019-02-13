using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace EventSourced.Framework.Abstracions
{
    public abstract class EventSourcedBase
    {

        public IEventSourcingSystem Context { get; set; }

        public abstract string PersistenceId { get; }

        public void Recover(IEnumerable<dynamic> history)
        {
            foreach (dynamic @event in history)
                DispatchToApply(@event);
        }

        public void Causes(object @event)
        {
            Persist(@event);
        }

        public async void Persist(object @event)
        {
            var persistSuccessful = Context.Save(PersistenceId, @event).GetAwaiter().GetResult();
            if (persistSuccessful)
                DispatchToApply(@event);
        }
        
        public void DispatchToApply(dynamic @event)
        {
            ((dynamic)this).Apply((dynamic)@event);
        }
    }
}
