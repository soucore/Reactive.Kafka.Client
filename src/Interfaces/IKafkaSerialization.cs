namespace Reactive.Kafka.Interfaces
{
    public interface IKafkaSerialization<T> : IKafkaSerialization
    {
        /// <summary>
        /// Get or handle message before serialization process
        /// </summary>
        /// <param name="rawMessage">Virgin message</param>
        /// <returns></returns>
        string OnBeforeSerialization(string rawMessage);


        /// <summary>
        /// Get or handle message after the serialization process 
        /// and before emitting the event to the consumer.
        /// </summary>
        /// <param name="message">serialized object</param>
        /// <returns></returns>
        T OnAfterSerialization(T message);
    }

    public interface IKafkaSerialization { }
}
