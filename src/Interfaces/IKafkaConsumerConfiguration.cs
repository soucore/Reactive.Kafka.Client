namespace Reactive.Kafka.Interfaces;

public interface IKafkaConsumerConfiguration
{
    /// <summary>
    ///     Called once, for each consumer instance,
    ///     during the consumer setup process.
    /// </summary>
    /// <param name="configuration">
    ///     Consumer configuration instance.
    /// </param>
    void OnConsumerConfiguration(ConsumerConfig configuration);
}
