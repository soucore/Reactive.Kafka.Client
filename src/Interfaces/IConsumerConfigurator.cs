namespace Reactive.Kafka.Interfaces;

public interface IConsumerConfigurator : IEnumerable<IConsumerWrapper>
{
    void AddConsumerPerPartition<T, TMessage>(string bootstrapServer, string topic, string groupId = default) where T : IKafkaConsumer<TMessage>;
    void AddConsumerPerPartition<T, TMessage>(string bootstrapServer, Action<IServiceProvider, KafkaConfiguration> setupAction) where T : IKafkaConsumer<TMessage>;
    void AddConsumerPerQuantity<T, TMessage>(string bootstrapServer, int quantity, string topic, string groupId = default) where T : IKafkaConsumer<TMessage>;
    void AddConsumerPerQuantity<T, TMessage>(string bootstrapServer, int quantity, Action<IServiceProvider, KafkaConfiguration> setupAction) where T : IKafkaConsumer<TMessage>;
}
