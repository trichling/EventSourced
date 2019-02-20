using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventSourced.Framework.Abstracions
{

    public interface IEventStore
    {

        long StoreVersion();

        IDisposable CatchUpSubscription(long lastPosition, Action<string, dynamic, long> eventHandler, Action hasCaughtUp);

        Task<IEnumerable<dynamic>> GetHistory(string persistenceId);

        Task<bool> Persist(string persistenceId, object @event);
        
       

    }

}