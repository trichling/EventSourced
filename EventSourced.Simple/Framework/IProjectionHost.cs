using System.Threading.Tasks;

namespace EventSourced.Simple.Framework
{
    public interface IProjectionHost
    {
        Task Subscribe();
        void Unsubscribe();
    }
}