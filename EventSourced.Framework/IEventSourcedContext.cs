using System; 
using System.Linq.Expressions; 
using System.Threading.Tasks; 

namespace EventSourced.Framework {
    public interface IEventSourcedContext {

        IEventStream EventStream {get; }
        IAllPersistenceIdsProjection AllStreams {get; }

        Task<T> Get<T> (Expression<Func<T>> factory, params object[] args)where T:EventSourcedBase; 

        Task<bool> Persist(string persistenceId, object @event);

    }
}