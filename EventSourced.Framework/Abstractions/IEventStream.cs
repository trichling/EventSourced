using System;

namespace EventSourced.Framework.Abstractions
{

    public interface IEventStream
    {

        void Subscribe<T>(EventHandlerCallback handler);
        void Publish(IEvent @event);
        
    }

}