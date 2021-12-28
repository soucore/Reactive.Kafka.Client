using Confluent.Kafka;

namespace Reactive.Kafka.Interfaces
{
    public interface IKafkaConsumer<T>
    {
        void OnConsumerConfiguration(IConsumer<string, string> consumer);
        void Consume(object sender, KafkaEventArgs<T> @event);
    }

}
