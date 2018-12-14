using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace EventSourced.Simple.Framework
{
    public abstract class EventSourcedBase
    {

        private List<object> _uncomittedEvents;

        public EventSourcedBase()
        {
            _uncomittedEvents = new List<object>();
        }

        public abstract string PersistenceId { get; }

        public IReadOnlyCollection<object> UncomittedEvents => new ReadOnlyCollection<object>(_uncomittedEvents);

        public void Commit()
        {
            _uncomittedEvents.Clear();
        }

        public void Causes(object @event)
        {
            _uncomittedEvents.Add(@event);
            DispatchToApply(@event);
        }

        public void OnRecover(dynamic @event)
        {
            DispatchToApply(@event);
        }

        public void DispatchToApply(dynamic @event)
        {
            ((dynamic)this).Apply((dynamic)@event);
        }

        public void Apply(object @event)
        {
            // catch all
        }
    }
}