using System;

namespace EventSourced.Framework
{

    public interface IEventStream
    {
        void Subscribe<T>(Action<T> handler);

        void Publish(object @event);
    }

}