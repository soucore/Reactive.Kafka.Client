using Confluent.Kafka;
using System.Threading.Tasks;

namespace Reactive.Kafka.Interfaces
{
    public interface IKafkaConsumer<T>
    {
        /// <summary>
        /// Entry point for each kafka message received.
        /// </summary>
        /// <param name="sender">Kafka consumer object for analysis purpose</param>
        /// <param name="event">Message received from broker</param>
        Task Consume(object sender, KafkaMessage<T> kafkaMessage, Commit commit);
        void OnConsumerConfiguration(IConsumer<string, string> consumer);
    }
}
