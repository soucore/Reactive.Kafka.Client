namespace Reactive.Kafka.Interfaces
{
    public interface IKafkaConsumerBuilder
    {
        /// <summary>
        /// Consumer builder.
        /// </summary>
        /// <param name="builder">Consumer builder instance</param>
        void OnConsumerBuilder(ConsumerBuilder<string, string> builder);
    }
}
