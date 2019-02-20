namespace EventSourced.Framework.Abstractions
{
    public class Event : IEvent
    {

        public Event(string persistenceId, long? position, object payload)
        {
            PersistenceId = persistenceId;
            Position = position;
            Payload = payload;
        }

        public long? Position { get; set; }

        public string PersistenceId { get; set; }

        public object Payload { get; set; }
    }
}