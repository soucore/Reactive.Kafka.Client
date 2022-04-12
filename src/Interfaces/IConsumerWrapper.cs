namespace Reactive.Kafka.Interfaces
{
    public interface IConsumerWrapper
    {
        IConsumer<string, string> Consumer { get; }
        DateTime LastConsume { get; }
    }
}
