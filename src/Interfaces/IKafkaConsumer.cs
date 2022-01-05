using Confluent.Kafka;

namespace Reactive.Kafka.Interfaces
{
    public interface IKafkaConsumer<T>
    {
        /// <summary>
        /// Entry point for each kafka message received.
        /// </summary>
        /// <param name="sender">Kafka consumer object for analysis purpose</param>
        /// <param name="event">Message received from broker</param>
        void Consume(object sender, KafkaEventArgs<T> @event);
        void OnConsumerConfiguration(IConsumer<string, string> consumer);
    }
}
