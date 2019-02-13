using System;

namespace EventSourced.Framework.Abstracions
{

    public interface IEventStream
    {

        void Subscribe<T>(Action<T> handler);

        void Publish(object @event);
    }

}