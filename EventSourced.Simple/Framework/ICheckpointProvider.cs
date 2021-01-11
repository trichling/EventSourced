using EventStore.Client;

namespace EventSourced.Simple.Framework
{

    public interface ICheckpointProvider
    {

        ulong? Get();

    }

    public class NoCheckpoinProvider : ICheckpointProvider
    {
        public ulong? Get() => null;
    }

    public static class CheckpointExtensions
    {

        // public Position AsPosition(this ICheckpointProvider checkpointProvider)
        // {
        //     var checkpoint = checkpointProvider.Get();
        //     return new Position()
        // }

        // public StreamPosition AsPosition(this ICheckpointProvider checkpointProvider)
        // {

        // }

    }

}