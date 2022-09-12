namespace Reactive.Kafka
{
    internal sealed class ProducerWrapperBuilder<T>
    {
        private readonly object consumerObj;
        private readonly IServiceProvider provider;
        private readonly ProducerConfig configuration = new();

        public ProducerWrapperBuilder(object consumerObj, IServiceProvider provider)
        {
            this.consumerObj = consumerObj;
            this.provider = provider;
        }

        public ProducerWrapper Build()
        {
            CallOnProducerBuilder();

            if (string.IsNullOrEmpty(configuration.BootstrapServers))
                return null;

            var producerBuilder = new ProducerBuilder<string, string>(configuration);
            var producer = producerBuilder.Build();

            return provider.CreateInstance<ProducerWrapper>(producer);
        }

        public void CallOnProducerBuilder()
        {
            typeof(T)
                .GetMethod("OnProducerConfiguration")?
                .Invoke(consumerObj, new object[] { configuration });
        }
    }
}
