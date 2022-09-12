namespace Reactive.Kafka.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddReactiveKafka(this IServiceCollection services, Action<IServiceProvider, IConsumerConfigurator> setupAction)
    {
        if (services.Any(d => d.ServiceType == typeof(IConsumerConfigurator)))
        {
            throw new InvalidOperationException(
                "AddReactiveKafka() was already called and may only be called once per container.");
        }

        services.TryAddTransient<IKafkaAdmin, KafkaAdmin>();
        services.TryAddSingleton<IList<IConsumerWrapper>, List<IConsumerWrapper>>();
        services.TryAddSingleton(provider =>
        {
            var consumerWrapperCollection = provider.GetRequiredService<IList<IConsumerWrapper>>();

            IConsumerConfigurator configurator = new ConsumerConfigurator(consumerWrapperCollection, provider);
            setupAction?.Invoke(provider, configurator);

            return configurator;
        });
    }
}