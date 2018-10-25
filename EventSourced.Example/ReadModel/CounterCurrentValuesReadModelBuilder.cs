using System;
using System.Collections.Generic;
using EventSourced.Framework;
using EventSourced.Example.Aggregate.Events;

namespace EventSourced.Example.Example.ReadModel
{

    public class CounterCurrentValuesReadModelBuilder
    {

        private Dictionary<Guid, int> counterValues = new Dictionary<Guid, int>();

        public CounterCurrentValuesReadModelBuilder(IEventSourcedContext context)
        {
            context.EventStream.Subscribe<CounterIntitialized>(HandleCounterInitialized);
            context.EventStream.Subscribe<CounterIncremented>(HandleCounterIncremented);
            context.EventStream.Subscribe<CounterDecremented>(HandleCounterDecremented);
        }

        
        public void HandleCounterInitialized(CounterIntitialized counterInitialized)
        {
            counterValues.Add(counterInitialized.CounterId, counterInitialized.InitialValue);
        }

        public void HandleCounterIncremented(CounterIncremented counterIncremented)
        {
            counterValues[counterIncremented.CounterId] += counterIncremented.ByValue;
        }

         public void HandleCounterDecremented(CounterDecremented counterDecremented)
        {
            counterValues[counterDecremented.CounterId] -= counterDecremented.ByValue;
        }
    }


}