using System; 
using System.Linq.Expressions; 
using System.Threading.Tasks; 

namespace EventSourced.Framework.Abstractions 
{
    public interface IEventSourcingSystem 
    {

        IEventStore EventStore { get; }

        IEventStream EventStream {get; }

        Task<T> Get<T> (Func<T> factory) where T : EventSourcedBase; 

        Task Save(string persistenceId, object @event);

        Task<bool> Commit(string persistenceId);

    }
}