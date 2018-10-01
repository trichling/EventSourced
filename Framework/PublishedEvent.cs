using MediatR;

namespace Lab.SqlStreamStoreDemo.Framework
{
    public class PublishedEvent : INotification
    {

        public PublishedEvent(string eventType, object @event)
        {
            EventType = eventType;
            Event = @event;
        }

        public string EventType { get; }
        public object Event { get; set; }

    }
}
