using Confluent.Kafka;
using System.Threading.Tasks;

namespace Reactive.Kafka.Interfaces
{
    public interface IKafkaConsumer<T>
    {
        /// <summary>
        /// Entry point for each kafka message received.
        /// </summary>
        /// <param name="kafkaMessage">Kafka message containing key and value</param>
        /// <param name="commit">Offset commit function</param>
        /// <returns></returns>
        Task Consume(ConsumerMessage<T> consumerMessage, Commit commit);

        /// <summary>
        /// Consumer configuration.
        /// </summary>
        /// <param name="consumer">Consumer instance</param>
        void OnConsumerConfiguration(IConsumer<string, string> consumer);
    }
}
