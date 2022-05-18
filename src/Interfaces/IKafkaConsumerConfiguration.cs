namespace Reactive.Kafka.Interfaces
{
    public interface IKafkaConsumerConfiguration
    {
        /// <summary>
        /// Consumer configuration.
        /// </summary>
        /// <param name="consumer">Consumer instance</param>
        void OnConsumerConfiguration(IConsumer<string, string> consumer);
    }
}
