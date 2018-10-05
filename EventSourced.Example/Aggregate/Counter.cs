using System;
using Lab.SqlStreamStoreDemo.ExampleAggregate.Commands;
using Lab.SqlStreamStoreDemo.ExampleAggregate.Events;
using Lab.SqlStreamStoreDemo.Framework;

namespace Lab.SqlStreamStoreDemo.ExampleAggregate
{
    public class Counter : EventSourced
    {
        private Guid _id;
        private int _counter;

        public override string PersistenceId => $"Counter-{_id}";

        public Counter(Guid id)
        {
            _id = id;
        }

        public void Handle(InitializeCounter initializeCounter)
        {
            Causes(new CounterIntitialized(_id, initializeCounter.InitialValue));
        }

        public void Apply(CounterIntitialized counterIntitialized)
        {
            _id = counterIntitialized.CounterId;
            _counter = counterIntitialized.InitialValue;
        }


        public void Handle(IncrementCounter incrementCounter)
        {
            Causes(new CounterIncremented(_id, incrementCounter.ByValue));
        }

        public void Apply(CounterIncremented counterIncremented)
        {
            _counter += counterIncremented.ByValue;
        }


        public void Handle(DecrementCounter decrementCounter)
        {
            Causes(new CounterDecremented(_id, decrementCounter.ByValue));
        }

        public void Apply(CounterDecremented counterDecremented)
        {
            _counter -= counterDecremented.ByValue;
        }
    }
}
