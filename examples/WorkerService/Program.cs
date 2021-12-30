using Reactive.Kafka.Extensions;
using WorkerService;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<Worker>();

        services.AddReactiveKafkaConsumer(config =>
        {
            config.BootstrapServers = "localhost:9092";
        });
    })
    .Build();

await host.RunAsync();
