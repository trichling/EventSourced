using System;

namespace Lab.SqlStreamStoreDemo.Framework
{

    public interface IEventStream
    {
        void Subscribe<T>(Action<T> handler);

        void Publish(object @event);
    }

}