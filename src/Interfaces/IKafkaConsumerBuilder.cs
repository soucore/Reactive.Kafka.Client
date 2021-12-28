using Confluent.Kafka;

namespace Reactive.Kafka.Interfaces
{
    public interface IKafkaConsumerBuilder
    {
        void OnConsumerBuilder(ConsumerConfig config);
    }
}
