using Reactive.Kafka.Extensions;
using UsingAbstractClass.Consumers;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddReactiveKafka((provider, configurator) =>
        {
            configurator.AddConsumerPerQuantity<Consumer2, string>("localhost:9092", quantity: 2, (provider, config) =>
            {
                config.RespectObjectContract = true;
                config.Topic = "your-topic";
                config.ConsumerConfig.GroupId = "your-group";
            });
        });
    })
    .Build();

await host.RunConsumersAsync();
await host.RunAsync();
