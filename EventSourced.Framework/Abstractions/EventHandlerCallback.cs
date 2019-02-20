namespace EventSourced.Framework.Abstractions
{

    public delegate void EventHandlerCallback(string persistenceId, long position, object payload);

}