using System;

namespace EventSourced.Simple.Aggregate.Events
{
    public class CounterIncremented
    {

        public CounterIncremented(Guid counterId, int byValue)
        {
            CounterId = counterId;
            ByValue = byValue;
        }

        public Guid CounterId { get; }
        public int ByValue { get; set; }
    }
}
