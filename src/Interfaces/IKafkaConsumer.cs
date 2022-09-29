namespace Reactive.Kafka.Interfaces;

public interface IKafkaConsumer<T> : IKafkaConsumer
{
    /// <summary>
    ///     Entry point for each kafka message received,
    ///     may not occur if serilization fails.
    /// </summary>
    /// <param name="kafkaMessage">
    ///     Kafka message containing key and value.
    /// </param>
    /// <param name="commit">
    ///     Commit function.
    /// </param>
    Task OnConsume(ConsumerMessage<T> consumerMessage, Commit commit);
}

public interface IKafkaConsumer { }
