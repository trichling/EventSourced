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

            system.EventStream.Subscribe<CounterIntitialized>(Handle);
            system.EventStream.Subscribe<CounterIncremented>(Handle);
            system.EventStream.Subscribe<CounterDecremented>(Handle);
        }

        public void Handle(CounterIntitialized counterInitialized)
        {
            counterValues.Add(counterInitialized.CounterId, counterInitialized.InitialValue);
        }

        public void Handle(CounterIncremented counterIncremented)
        {
            counterValues[counterIncremented.CounterId] += counterIncremented.ByValue;
        }

         public void Handle(CounterDecremented counterDecremented)
        {
            counterValues[counterDecremented.CounterId] -= counterDecremented.ByValue;
        }
    }


}