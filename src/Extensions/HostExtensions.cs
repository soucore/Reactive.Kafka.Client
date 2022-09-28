namespace Reactive.Kafka.Extensions;

public static class HostExtensions
{
    public static Task RunConsumersAsync(this IHost host)
    {
        var stoppingToken = default(CancellationToken);

        var hostApplicationLifetime = host.Services.GetService<IHostApplicationLifetime>();
        if (hostApplicationLifetime != null)
            stoppingToken = hostApplicationLifetime.ApplicationStopping;

        host.Services.RunConsumers(stoppingToken);

        return Task.CompletedTask;
    }
}
