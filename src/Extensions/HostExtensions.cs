namespace Reactive.Kafka.Extensions;

public static class HostExtensions
{
    public static Task RunConsumersAsync(this IHost host)
    {
        host.Services.RunConsumers();
        return Task.CompletedTask;
    }
}
