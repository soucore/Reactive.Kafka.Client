namespace Reactive.Kafka.Interfaces;

public interface IKafkaConsumerBuilder
{
    /// <summary>
    ///     Called once, for each consumer instance,
    ///     before the kafka consumer is built.
    /// </summary>
    /// <param name="builder">
    ///     Consumer builder instance.
    /// </param>
    void OnConsumerBuilder(ConsumerBuilder<string, string> builder);
}
