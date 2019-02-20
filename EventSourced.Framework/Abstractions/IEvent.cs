namespace EventSourced.Framework.Abstractions
{

    public interface IEvent
    {
        
        long? Position { get; }

        string PersistenceId { get; }

        object Payload { get; }

    }

}