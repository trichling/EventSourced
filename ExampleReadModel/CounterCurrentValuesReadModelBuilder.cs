using System;
using System.Collections.Generic;
using Lab.SqlStreamStoreDemo.ExampleAggregate.Events;
using Lab.SqlStreamStoreDemo.Framework;

namespace Lab.SqlStreamStoreDemo.ExampleReadModel
{

    public class CounterCurrentValuesReadModelBuilder
    {

        private Dictionary<Guid, int> counterValues = new Dictionary<Guid, int>();

        public CounterCurrentValuesReadModelBuilder(DomainContext context)
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