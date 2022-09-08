using Reactive.Kafka.Extensions;
using UsingInterfaces.Consumers;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddReactiveKafka((provider, configurator) =>
        {
            configurator.AddConsumerPerPartition<Consumer1, string>("localhost:9092", "your-topic");
        });
    })
    .Build();

await host.RunConsumersAsync();
await host.RunAsync();
