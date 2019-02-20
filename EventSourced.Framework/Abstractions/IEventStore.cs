using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventSourced.Framework.Abstractions
{

    public interface IEventStore
    {

        long StoreVersion();

        IDisposable CatchUpSubscription(long lastPosition, EventHandlerCallback eventHandler, Action hasCaughtUp);

        Task<IEnumerable<dynamic>> GetHistory(string persistenceId);

        Task<long> Persist(string persistenceId, object @event);
        
       

    }

}