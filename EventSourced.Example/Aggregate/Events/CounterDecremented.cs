using System;

namespace EventSourced.Example.Aggregate.Events
{
    public class CounterDecremented
    {

        public CounterDecremented(Guid counterId, int byValue)
        {
            CounterId = counterId;
            ByValue = byValue;
        }

        public Guid CounterId { get; }
        public int ByValue { get; set; }
    }
}
