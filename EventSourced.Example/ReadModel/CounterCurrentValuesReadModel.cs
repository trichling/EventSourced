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
            system.EventStream.Subscribe<CounterIntitialized>(Handle);
            system.EventStream.Subscribe<CounterIncremented>(Handle);
            system.EventStream.Subscribe<CounterDecremented>(Handle);

            this.CatchUp().GetAwaiter().GetResult();
        }
      

        public void Handle(string persistenceId, CounterIntitialized counterInitialized)
        {
            counterValues.Add(counterInitialized.CounterId, counterInitialized.InitialValue);
        }

        public void Handle(string persistenceId, CounterIncremented counterIncremented)
        {
            counterValues[counterIncremented.CounterId] += counterIncremented.ByValue;
        }

         public void Handle(string persistenceId, CounterDecremented counterDecremented)
        {
            counterValues[counterDecremented.CounterId] -= counterDecremented.ByValue;
        }
    }


}