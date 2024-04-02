using System.Collections;

namespace Reactive.Kafka;

public class ConsumerConfigurator(IList<IConsumerWrapper> consumerWrapperCollection, IServiceProvider provider) : IConsumerConfigurator
{
    public void AddConsumerPerPartition<T, TMessage>(string bootstrapServer, string topic, string groupId = default)
        where T : IKafkaConsumer<TMessage>
    {
        AddConsumerPerPartition<T, TMessage>(bootstrapServer, (provider, config) =>
        {
            config.Topic = topic;
            config.ConsumerConfig.GroupId = groupId;
            config.ConsumerConfig.BootstrapServers = bootstrapServer;
        });
    }

    public void AddConsumerPerPartition<T, TMessage>(string bootstrapServer, Action<IServiceProvider, KafkaConfiguration> setupAction)
        where T : IKafkaConsumer<TMessage>
    {
        ArgumentNullException.ThrowIfNull(bootstrapServer);
        ArgumentNullException.ThrowIfNull(setupAction);

        KafkaConfiguration config;

        config = new KafkaConfiguration();
        config.ConsumerConfig.BootstrapServers = bootstrapServer;

        setupAction(provider, config);

        if (string.IsNullOrEmpty(config.Topic))
        {
            throw new ArgumentException("Topic cannot be null or empty string");
        }

        config.ConsumerConfig.GroupId ??= Guid.NewGuid().ToString();

        KafkaBuilder.BuildConsumerPerPartition<T, TMessage>(provider, config);
    }

    public void AddConsumerPerQuantity<T, TMessage>(string bootstrapServer, int quantity, string topic, string groupId = null)
        where T : IKafkaConsumer<TMessage>
    {
        AddConsumerPerQuantity<T, TMessage>(bootstrapServer, quantity, (provider, config) =>
        {
            config.Topic = topic;
            config.ConsumerConfig.GroupId = groupId;
            config.ConsumerConfig.BootstrapServers = bootstrapServer;
        });
    }

    public void AddConsumerPerQuantity<T, TMessage>(string bootstrapServer, int quantity, Action<IServiceProvider, KafkaConfiguration> setupAction)
        where T : IKafkaConsumer<TMessage>
    {
        ArgumentNullException.ThrowIfNull(bootstrapServer);
        ArgumentNullException.ThrowIfNull(setupAction);

        KafkaConfiguration config;

        config = new KafkaConfiguration();
        config.ConsumerConfig.BootstrapServers = bootstrapServer;

        setupAction(provider, config);

        if (string.IsNullOrEmpty(config.Topic))
        {
            throw new ArgumentException("Topic cannot be null or empty string");
        }

        config.ConsumerConfig.GroupId ??= Guid.NewGuid().ToString();

        KafkaBuilder.BuildConsumerPerQuantity<T, TMessage>(quantity, provider, config);
    }

    public IEnumerator<IConsumerWrapper> GetEnumerator()
    {
        return consumerWrapperCollection.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
