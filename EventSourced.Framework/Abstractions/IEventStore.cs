using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventSourced.Framework.Abstracions
{

    public interface IEventStore
    {

        IAllPersistenceIdsProjection AllPersistenceIdsProjection { get; }

        Task<IEnumerable<dynamic>> GetHistory(string persistenceId);

        Task<bool> Persist(string persistenceId, object @event);
        
    }

}