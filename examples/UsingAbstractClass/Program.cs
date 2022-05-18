using Reactive.Kafka.Extensions;
using UsingAbstractClass;
using UsingAbstractClass.Consumers;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<Worker>();

        services.AddReactiveKafkaConsumerPerQuantity<Consumer2>("localhost:9092", quantity: 2, topic: "your-another-topic");
        services.AddReactiveKafkaConsumerPerQuantity<Consumer1>(quantity: 2, config =>
        {
            config.RespectObjectContract = true;
            config.Topic = "your-topic";
            config.ConsumerConfig.BootstrapServers = "localhost:9092";
            config.ConsumerConfig.GroupId = "your-group";
        });
    })
    .Build();

await host.RunAsync();
