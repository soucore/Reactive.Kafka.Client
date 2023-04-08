using Confluent.Kafka;
using Reactive.Kafka.Extensions;
using UsingAbstractClass;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddReactiveKafka((provider, configurator) =>
        {
            configurator.AddConsumerPerQuantity<Consumer, string>("localhost:9092", quantity: 2, (provider, cfg) =>
            {
                cfg.Topic = "your-topic";
                cfg.ConsumerConfig.GroupId = "your-group";
                cfg.ConsumerConfig.AutoCommitIntervalMs = 0;
                cfg.ConsumerConfig.EnableAutoCommit = false;
                cfg.ConsumerConfig.AutoOffsetReset = AutoOffsetReset.Latest;
            });
        });
    })
    .Build();

await host.RunConsumersAsync();
await host.RunAsync();
