namespace Reactive.Kafka;

internal sealed class ProducerWrapperBuilder<T>(object consumerObj, IServiceProvider provider)
{
    private readonly ProducerConfig configuration = new();

    public ProducerWrapper Build()
    {
        InvokeOnProducerConfiguration();

        if (string.IsNullOrEmpty(configuration.BootstrapServers))
            return null;

        var producerBuilder = new ProducerBuilder<string, string>(configuration);

        InvokeOnProducerBuilder(producerBuilder);

        return provider.CreateInstance<ProducerWrapper>(producerBuilder.Build());
    }

    public void InvokeOnProducerConfiguration()
    {
        typeof(T)
            .GetMethod("OnProducerConfiguration")?
            .Invoke(consumerObj, [configuration]);
    }

    public void InvokeOnProducerBuilder(ProducerBuilder<string, string> builder)
    {
        typeof(T)
            .GetMethod("OnProducerBuilder")?
            .Invoke(consumerObj, [builder]);
    }
}
