using System.Threading.Tasks;

namespace EventSourced.Framework.Abstracions
{

    public interface IReadModel
    {

        bool IsUpToDate { get; }

        Task CatchUp();
        
    }

}