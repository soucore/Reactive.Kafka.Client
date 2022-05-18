namespace Reactive.Kafka.Interfaces
{
    internal interface IConsumerConfiguration
    {
        /// <summary>
        /// Consumer configuration.
        /// </summary>
        /// <param name="consumer">Consumer instance</param>
        void OnConsumerConfiguration(IConsumer<string, string> consumer);
    }
}
