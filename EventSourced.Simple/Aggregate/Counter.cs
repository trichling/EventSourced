using System;
using EventSourced.Simple.Aggregate.Events;
using EventSourced.Simple.Framework;

namespace EventSourced.Simple.Aggregate
{
    public class Counter : EventSourcedBase
    {
        private Guid _id;
        private int? _counter;

        public override string PersistenceId => $"SimpleCounter-{_id}";

        public Counter(Guid id)
        {
            _id = id;
        }

        public void Initialize(int initialValue)
        {
            if (_counter != null)
                throw new Exception("Counter ist bereits initialisiert");

            if (initialValue < 0)
                throw new Exception("Counter darf nicht negativ werden");

            Causes(new CounterIntitialized(_id, initialValue));
        }

        public void Apply(CounterIntitialized counterIntitialized)
        {
            _counter = counterIntitialized.InitialValue;
        }


        public void Increment(int byValue)
        {
            if (_counter == null)
                throw new Exception("Counter ist nicht initialisiert");

            Causes(new CounterIncremented(_id, byValue));
        }

        public void Apply(CounterIncremented counterIncremented)
        {
            _counter += counterIncremented.ByValue;
        }


        public void Decrement(int byValue)
        {
            if (_counter == null)
                throw new Exception("Counter ist nicht initialisiert");
                
            if (_counter < byValue)
                throw new Exception("Counter darf nicht negativ werden");

            Causes(new CounterDecremented(_id, byValue));
        }

        public void Apply(CounterDecremented counterDecremented)
        {
            _counter -= counterDecremented.ByValue;
        }
    }
}
