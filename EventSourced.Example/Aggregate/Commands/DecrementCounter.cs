namespace Lab.SqlStreamStoreDemo.ExampleAggregate.Commands
{
    public class DecrementCounter
    {

        public DecrementCounter(int byValue)
        {
            ByValue = byValue;
        }

        public int ByValue { get; set; }
    }
}
