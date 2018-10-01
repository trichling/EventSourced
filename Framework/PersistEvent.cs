using MediatR;

namespace Lab.SqlStreamStoreDemo.Framework
{
    public class PersistEvent : IRequest<bool>
    {
        public PersistEvent(string persistenceId, object @event)
        {
            PersistenceId = persistenceId;
            Event = @event;
        }

        public string PersistenceId { get; }
        public object Event { get; set; }
    }
}
