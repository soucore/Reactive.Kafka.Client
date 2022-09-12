namespace Reactive.Kafka.Interfaces;

public interface IKafkaConsumerConfiguration
{
    /// <summary>
    /// Consumer configuration.
    /// </summary>
    /// <param name="configuration">Configuration object</param>
    void OnConsumerConfiguration(ConsumerConfig configuration);
}
