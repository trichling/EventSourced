namespace EventSourced.Example.Aggregate.Commands
{
    public class InitializeCounter
    {

        public InitializeCounter(int initialValue)
        {
            InitialValue = initialValue;
        }

        public int InitialValue { get; set; }

    }
}
