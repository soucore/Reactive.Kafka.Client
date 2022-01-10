using Confluent.Kafka;

namespace Reactive.Kafka.Interfaces
{
    public interface IConsumerWrapper
    {
        IConsumer<string, string> Consumer { get; }
    }
}
