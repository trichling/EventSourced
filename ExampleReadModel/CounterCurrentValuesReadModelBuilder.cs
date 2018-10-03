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
            Context = context;

            Context.EventStream.Subscribe<CounterIntitialized>(HandleCounterInitialized);
            Context.EventStream.Subscribe<CounterIncremented>(HandleCounterIncremented);
            Context.EventStream.Subscribe<CounterDecremented>(HandleCounterDecremented);
        }

        public DomainContext Context { get; }
        
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