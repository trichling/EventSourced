namespace EventSourced.Example.Aggregate.Commands
{
    public class IncrementCounter
    {

        public IncrementCounter(int byValue)
        {
            ByValue = byValue;
        }

        public int ByValue { get; set; }
    }
}
