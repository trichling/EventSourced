using System;
using System.Collections.Generic;
using EventSourced.Framework;
using EventSourced.Example.Aggregate.Events;
using EventSourced.Framework.Abstractions;
using System.Threading.Tasks;

namespace EventSourced.Example.Example.ReadModel
{

    public class CounterCurrentValuesReadModel : ReadModelBase
    {

        private Dictionary<Guid, int> counterValues = new Dictionary<Guid, int>();

        public CounterCurrentValuesReadModel(IEventSourcingSystem system) : base(system)
        {
            // Da dies ein In-Memory-Readmodel ist, muss es immer von 0 aufholen wenn es neu erstellt wird.
            this.StartCatchingUpFrom(0);
            this.WaitForCatchUp().GetAwaiter().GetResult();

            system.EventStream.Subscribe<CounterIntitialized>(Apply);
            system.EventStream.Subscribe<CounterIncremented>(Apply);
            system.EventStream.Subscribe<CounterDecremented>(Apply);
        }

        public void Apply(CounterIntitialized counterInitialized)
        {
            counterValues.Add(counterInitialized.CounterId, counterInitialized.InitialValue);
        }

        public void Apply(CounterIncremented counterIncremented)
        {
            counterValues[counterIncremented.CounterId] += counterIncremented.ByValue;
        }

         public void Apply(CounterDecremented counterDecremented)
        {
            counterValues[counterDecremented.CounterId] -= counterDecremented.ByValue;
        }
    }


}