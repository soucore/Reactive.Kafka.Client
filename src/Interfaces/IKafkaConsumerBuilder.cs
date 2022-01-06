using Confluent.Kafka;

namespace Reactive.Kafka.Interfaces
{
    public interface IKafkaConsumerBuilder
    {
        /// <summary>
        /// Kafka consumer configuration.
        /// </summary>
        /// <param name="builder">Configuration object</param>
        void OnConsumerBuilder(ConsumerConfig builder);
    }
}
