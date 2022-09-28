using Confluent.Kafka;
using ConsumerPerPartition;
using Reactive.Kafka.Extensions;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddReactiveKafka((provider, configurator) =>
        {
            configurator.AddConsumerPerPartition<Consumer2, Message>("localhost:9092", "your-topic", "your-group");
            configurator.AddConsumerPerPartition<Consumer1, string>("localhost:9092", (provider, configuration) =>
            {
                configuration.Topic = "your-another-topic";
                configuration.ConsumerConfig.GroupId = "your-another-group";
                configuration.ConsumerConfig.AutoOffsetReset = AutoOffsetReset.Latest;
            });
        });
    })
    .Build();

await host.RunConsumersAsync();
await host.RunAsync();
