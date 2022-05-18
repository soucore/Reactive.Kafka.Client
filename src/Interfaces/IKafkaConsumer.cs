namespace Reactive.Kafka.Interfaces
{
    public interface IKafkaConsumer<T> : IKafkaConsumer
    {
        /// <summary>
        /// Entry point for each kafka message received.
        /// </summary>
        /// <param name="kafkaMessage">Kafka message containing key and value</param>
        /// <param name="commit">Offset commit function</param>
        /// <returns></returns>
        Task OnConsume(ConsumerMessage<T> consumerMessage, Commit commit);
    }

    public interface IKafkaConsumer { }
}
