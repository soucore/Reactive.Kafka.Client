namespace Reactive.Kafka.Interfaces;

public interface IKafkaConsumerError
{
    /// <summary>
    ///     Entry point for each message that could not
    ///     be serialized to desired type or any internal
    ///     kafka exception occurred.
    /// </summary>
    /// <param name="consumerError">
    ///     Object containing error and exception information.
    /// </param>
    /// <param name="commit">
    ///     Commit function.
    /// </param>
    Task OnConsumeError(KafkaConsumerError consumerError, Commit commit);
}
