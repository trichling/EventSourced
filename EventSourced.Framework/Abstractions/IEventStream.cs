using System;

namespace EventSourced.Framework.Abstracions
{

    public interface IEventStream
    {

        void Subscribe<T>(Action<string, T> handler);

        void Publish(string persistenceId, object @event);
    }

}