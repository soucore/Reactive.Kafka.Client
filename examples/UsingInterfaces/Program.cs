using Reactive.Kafka.Extensions;
using UsingInterfaces;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<Worker>();
        services.AddReactiveKafkaConsumer("localhost:9092");
    })
    .Build();

await host.RunAsync();
