using System;

namespace EventSourced.Framework.Abstractions
{

    public interface IEventStream
    {

        void Subscribe<T>(Action<T> handler);

        void Publish(object @event);
        
    }

}