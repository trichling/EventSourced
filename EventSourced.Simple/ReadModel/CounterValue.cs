using System;

namespace EventSourced.Simple.ReadModel
{
    public class CounterValue
    {
        public Guid Id { get; set; }

        public int Value { get; set; }
    }
}