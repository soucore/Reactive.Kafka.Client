namespace Reactive.Kafka.Interfaces;

public interface IKafkaSerialization<T> : IKafkaSerialization
{
    /// <summary>
    ///     Handles raw message before serialization process.
    /// </summary>
    /// <param name="rawMessage">
    ///     Raw message consumed from topic.
    /// </param>
    /// <returns>
    ///     Raw message to be forwarded for serialization.
    /// </returns>
    string OnBeforeSerialization(string rawMessage);

    /// <summary>
    ///     Handles serialized object after serialization process,
    ///     may not occur if serilization fails.
    /// </summary>
    /// <param name="message">
    ///     Serialized object.
    /// </param>
    /// <returns>
    ///     Serialized object to be forwarded to the OnConsume step.
    /// </returns>
    T OnAfterSerialization(T message);
}

public interface IKafkaSerialization { }
