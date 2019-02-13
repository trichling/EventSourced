using System;
using System.Collections.Generic;
using EventSourced.Framework;
using EventSourced.Example.Aggregate.Events;
using EventSourced.Framework.Abstracions;
using System.Threading.Tasks;

namespace EventSourced.Example.Example.ReadModel
{

    public class CounterCurrentValuesReadModel : ReadModelBase
    {

        private Dictionary<Guid, int> counterValues = new Dictionary<Guid, int>();

        public CounterCurrentValuesReadModel(IEventSourcingSystem system) : base(system)
        {
            this.CatchUp().GetAwaiter().GetResult();

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