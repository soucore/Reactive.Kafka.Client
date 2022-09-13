using Reactive.Kafka.Bindings;

namespace Reactive.Kafka;

internal static class KafkaBuilder
{
    public static void BuildConsumerPerPartition<T, TMessage>(IServiceProvider provider, KafkaConfiguration configuration)
    {
        ProducerWrapperBuilder<T> producerWrapperBuilder;
        ProducerWrapper producerWrapper = null;

        var consumerWrapperList = provider.GetRequiredService<IList<IConsumerWrapper>>();

        var kafkaAdmin = provider.GetRequiredService<IKafkaAdmin>();
        var partitions = kafkaAdmin.Partitions(configuration);

        foreach (int _ in Enumerable.Range(0, partitions))
        {
            var consumerObj = provider.CreateInstance<T>();
            var consumerWrapperBuilder = new ConsumerWrapperBuilder<T, TMessage>(consumerObj, configuration, provider);
            var consumerWrapper = consumerWrapperBuilder.Build();

            if (consumerWrapper is null)
                continue;

            var consumerBinding = new ConsumerBinding(consumerWrapper, consumerObj);

            consumerBinding.BindOnBeforeSerialization();
            consumerBinding.BindOnAfterSerialization();
            consumerBinding.BindOnConsume();
            consumerBinding.BindOnConsumeError();
            consumerBinding.BindOnFinish();

            producerWrapperBuilder = new ProducerWrapperBuilder<T>(consumerObj, provider);
            producerWrapper ??= producerWrapperBuilder.Build();

            if (producerWrapper is not null)
            {
                var producerBinding = new ProducerBinding(consumerObj, producerWrapper);

                producerBinding.BindOnProduce();
                producerBinding.BindOnProduceAsync();
            }

            consumerWrapperList.Add(consumerWrapper);
        }
    }

    public static void BuildConsumerPerQuantity<T, TMessage>(int quantity, IServiceProvider provider, KafkaConfiguration configuration)
    {
        ProducerWrapperBuilder<T> producerWrapperBuilder;
        ProducerWrapper producerWrapper = null;

        var consumerWrapperList = provider.GetRequiredService<IList<IConsumerWrapper>>();

        foreach (int _ in Enumerable.Range(0, quantity))
        {
            var consumerObj = provider.CreateInstance<T>();
            var consumerWrapperBuilder = new ConsumerWrapperBuilder<T, TMessage>(consumerObj, configuration, provider);
            var consumerWrapper = consumerWrapperBuilder.Build();

            if (consumerWrapper is null)
                continue;

            var consumerBinding = new ConsumerBinding(consumerWrapper, consumerObj);

            consumerBinding.BindOnBeforeSerialization();
            consumerBinding.BindOnAfterSerialization();
            consumerBinding.BindOnConsume();
            consumerBinding.BindOnConsumeError();
            consumerBinding.BindOnFinish();

            producerWrapperBuilder = new ProducerWrapperBuilder<T>(consumerObj, provider);
            producerWrapper ??= producerWrapperBuilder.Build();

            if (producerWrapper is not null)
            {
                var producerBinding = new ProducerBinding(consumerObj, producerWrapper);

                producerBinding.BindOnProduce();
                producerBinding.BindOnProduceAsync();
            }

            consumerWrapperList.Add(consumerWrapper);
        }
    }
}
