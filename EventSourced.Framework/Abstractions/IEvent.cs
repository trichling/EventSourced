namespace EventSourced.Framework.Abstractions
{

    public interface IEvent
    {

        string PersistenceId { get; }

        long? Position { get; }

        object Payload { get; }

    }
}