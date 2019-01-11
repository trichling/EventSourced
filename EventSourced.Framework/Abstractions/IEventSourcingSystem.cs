using System; 
using System.Linq.Expressions; 
using System.Threading.Tasks; 

namespace EventSourced.Framework.Abstracions 
{
    public interface IEventSourcingSystem 
    {

        IEventStore EventStore { get; }

        IEventStream EventStream {get; }

        Task<T> Get<T> (Func<T> factory) where T : EventSourcedBase; 

        Task<bool> Save(string persistenceId, dynamic @event);

        Task<bool> Commit(string persistenceId);

    }
}