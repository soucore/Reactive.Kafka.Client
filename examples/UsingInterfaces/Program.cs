using Reactive.Kafka.Extensions;
using UsingInterfaces;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddReactiveKafka((provider, configurator) =>
        {
            configurator.AddConsumerPerPartition<Consumer, string>("localhost:9092", "tenho-15", "grupo-tenho-15");
        });
    })
    .Build();

await host.RunConsumersAsync();
await host.RunAsync();
