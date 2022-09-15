namespace Reactive.Kafka
{
    internal sealed class ConsumerWrapperBuilder<T, TMessage>
    {
        private readonly object consumerObj;
        private readonly KafkaConfiguration configuration;
        private readonly IServiceProvider provider;

        public ConsumerWrapperBuilder(object consumerObj, 
            KafkaConfiguration configuration, 
            IServiceProvider provider)
        {
            this.consumerObj = consumerObj;
            this.configuration = configuration;
            this.provider = provider;
        }

        public ConsumerWrapper<TMessage> Build()
        {
            CallOnConsumerConfiguration();

            var builder = new ConsumerBuilder<string, string>(configuration.ConsumerConfig);

            CallOnConsumerBuilder(builder);

            var consumer = builder.Build();
            consumer.Subscribe(configuration.Topic);

            CallOnReady();

            return provider.CreateInstance<ConsumerWrapper<TMessage>>(consumer, configuration);
        }

        private void CallOnReady()
        {
            typeof(T)
                .GetMethod("OnReady")?
                .Invoke(consumerObj, Array.Empty<object>());
        }

        public void CallOnConsumerConfiguration()
        {
            typeof(T)
                .GetMethod("OnConsumerConfiguration")?
                .Invoke(consumerObj, new object[] { configuration.ConsumerConfig });
        }

        public void CallOnConsumerBuilder(ConsumerBuilder<string, string> builder)
        {
            typeof(T)
                .GetMethod("OnConsumerBuilder")?
                .Invoke(consumerObj, new object[] { builder });
        }
    }
}
