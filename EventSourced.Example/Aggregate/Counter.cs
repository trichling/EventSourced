using System;
using EventSourced.Framework;
using EventSourced.Example.Aggregate.Events;
using EventSourced.Framework.Abstracions;

namespace EventSourced.Example.Aggregate
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

        public bool IsInitialized()
        {
            return _counter != null;
        }

        public void Initialize(int initialValue)
        {
            if (IsInitialized())
                throw new Exception("Counter ist bereits initialisiert");

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
