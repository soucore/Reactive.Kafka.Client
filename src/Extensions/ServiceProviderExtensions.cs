namespace Reactive.Kafka.Extensions;

public static class ServiceProviderExtensions
{
    public static void RunConsumers(this IServiceProvider provider, CancellationToken stoppingToken = default)
    {
        var tasksToWait = new List<Task>();

        stoppingToken.Register(WaitForConsumersShutdown, tasksToWait);

        foreach (var consumerWrapper in provider.GetService<IConsumerConfigurator>())
        {
            TaskCompletionSource<object> taskCompletionSource = new(TaskCreationOptions.RunContinuationsAsynchronously);
            consumerWrapper.ConsumerStart(taskCompletionSource, stoppingToken);

            tasksToWait.Add(taskCompletionSource.Task);
        }
    }

    internal static void WaitForConsumersShutdown(object obj)
    {
        Task[] tasksToWait = ((IEnumerable<Task>)obj).ToArray();

        // block the calling thread until consumers
        // shutdown or timeout is reached.
        Task.WaitAll(tasksToWait, 60 * 1000);
    }

    public static T CreateInstance<T>(this IServiceProvider provider, params object[] parameters)
    {
        return (T)ActivatorUtilities.CreateInstance(provider, typeof(T), parameters);
    }
}