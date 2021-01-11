using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventSourced.Simple.Aggregate.Events;
using EventSourced.Simple.Framework;

namespace EventSourced.Simple.ReadModel
{
    public class CurrentCounterValuesReadModelBuilder : ReadModelBuilderBase
    {
        public Dictionary<Guid, int> CounterValues;

        public CurrentCounterValuesReadModelBuilder() 
            : base("CurrentCounterValues", "$ce-SimpleCounter")
        {
            CounterValues = new Dictionary<Guid, int>();
        }

        public Task Apply(CounterIntitialized e)
        {
            CounterValues.Add(e.CounterId, e.InitialValue);
            return Task.CompletedTask;
        }

        public Task Apply(CounterIncremented e)
        {
            CounterValues[e.CounterId] += e.ByValue;
            return Task.CompletedTask;
        }

        public Task Apply(CounterDecremented e)
        {
            CounterValues[e.CounterId] -= e.ByValue;
            return Task.CompletedTask;
        }
    }
}