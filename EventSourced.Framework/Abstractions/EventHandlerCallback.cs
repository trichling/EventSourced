namespace EventSourced.Framework.Abstractions
{

    public delegate void EventHandlerCallback(string persistenceId, long? position, object payload);
    public delegate void EventHandlerCallback<T>(string persistenceId, long? position, T payload);

}