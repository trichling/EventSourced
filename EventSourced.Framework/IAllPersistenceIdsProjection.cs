using System.Collections.ObjectModel;
using System.Threading.Tasks;
using SqlStreamStore.Streams;

namespace EventSourced.Framework
{
    public interface IAllPersistenceIdsProjection
    {

        bool IsUpToDate { get; }

        ReadOnlyCollection<StreamId> StreamIds { get; }

        Task WaitUntilIsUpToDate();
    }
}