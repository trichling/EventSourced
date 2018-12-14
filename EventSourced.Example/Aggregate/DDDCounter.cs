using System;
using EventSourced.Framework;
using EventSourced.Example.Aggregate.Commands;
using EventSourced.Example.Aggregate.Events;

namespace EventSourced.Example.Aggregate
{
    public class DDDCounter : EventSourcedBase
    {
        private Guid _id;
        private int _counter;

        public override string PersistenceId => $"Counter-{_id}";

        public DDDCounter(Guid id)
        {
            _id = id;
        }

        public void InitializeCounter(int initialValue)
        {
            Causes(new CounterIntitialized(_id, initialValue));
        }

        public void Apply(CounterIntitialized counterIntitialized)
        {
            _id = counterIntitialized.CounterId;
            _counter = counterIntitialized.InitialValue;
        }


        public void IncrementCounter(int byValue)
        {
            Causes(new CounterIncremented(_id, byValue));
        }

        public void Apply(CounterIncremented counterIncremented)
        {
            _counter += counterIncremented.ByValue;
        }


        public void DecrementCounter(int byValue)
        {
            Causes(new CounterDecremented(_id, byValue));
        }

        public void Apply(CounterDecremented counterDecremented)
        {
            _counter -= counterDecremented.ByValue;
        }
    }
}
