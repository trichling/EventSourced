using System.Threading.Tasks;

namespace EventSourced.Framework.Abstractions
{

    public interface IReadModel
    {

        long LastPosition { get; }

        bool IsUpToDate { get; }

        void StartCatchingUpFrom(long lastPosition);

        Task WaitForCatchUp();
        
    }

}