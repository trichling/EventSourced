using System.Threading.Tasks;

namespace EventSourced.Simple.Framework
{
    public interface IProjection
    {
        string Name { get; }

        string Stream { get; }

         Task Handle(dynamic e);
    }
}