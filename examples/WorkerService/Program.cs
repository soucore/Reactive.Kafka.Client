using Reactive.Kafka.Extensions;
using WorkerService;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<Worker>();

        services.AddReactiveKafkaConsumer(config =>
        {
            config.BootstrapServers = "192.168.29.10:9092";
        });


    })
    .Build();

await host.RunAsync();
