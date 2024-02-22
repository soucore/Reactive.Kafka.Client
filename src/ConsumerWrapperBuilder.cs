namespace Reactive.Kafka
{
    internal sealed class ConsumerWrapperBuilder<T, TMessage>
    {
        private readonly object consumerObj;
        private readonly KafkaConfiguration configuration;
        private readonly IServiceProvider provider;

        public ConsumerWrapperBuilder(object consumerObj, KafkaConfiguration configuration, IServiceProvider provider)
        {
            this.consumerObj = consumerObj;
            this.configuration = configuration;
            this.provider = provider;
        }

        public ConsumerWrapper<TMessage> Build()
        {
            InvokeOnConsumerConfiguration();

            var builder = new ConsumerBuilder<string, string>(configuration.ConsumerConfig);

            InvokeOnConsumerBuilder(builder);

            var consumer = builder.Build();
            consumer.Subscribe(configuration.Topic);

            InvokeOnReady();

            return provider.CreateInstance<ConsumerWrapper<TMessage>>(consumer, configuration);
        }

        private void InvokeOnReady()
            => typeof(T)
                .GetMethod("OnReady")?
                .Invoke(consumerObj, []);

        public void InvokeOnConsumerConfiguration()
            => typeof(T)
                .GetMethod("OnConsumerConfiguration")?
                .Invoke(consumerObj, [configuration.ConsumerConfig]);

        public void InvokeOnConsumerBuilder(ConsumerBuilder<string, string> builder)
            => typeof(T)
                .GetMethod("OnConsumerBuilder")?
                .Invoke(consumerObj, [builder]);
    }
}
