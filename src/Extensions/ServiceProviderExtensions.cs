namespace Reactive.Kafka.Extensions;

public static class ServiceProviderExtensions
{
    public static void RunConsumers(this IServiceProvider provider)
    {
        foreach (var consumerWrapper in provider.GetService<IConsumerConfigurator>())
            consumerWrapper.ConsumerStart();
    }

    public static T CreateInstance<T>(this IServiceProvider provider, params object[] parameters)
    {
        return (T)ActivatorUtilities.CreateInstance(provider, typeof(T), parameters);
    }
}
