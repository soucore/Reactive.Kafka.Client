using ConsumerPerPartition;
using Reactive.Kafka.Extensions;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<Worker>();
        services.AddReactiveKafkaConsumerPerPartition<Consumer1>("localhost:9092");
    })
    .Build();

await host.RunAsync();
