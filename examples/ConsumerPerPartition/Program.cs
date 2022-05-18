using ConsumerPerPartition;
using Reactive.Kafka.Extensions;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<Worker>();
        services.AddReactiveKafkaConsumerPerPartition<Consumer1>("localhost:9092", topic: "your-topic");
        services.AddReactiveKafkaConsumerPerPartition<Consumer2>(config =>
        {
            config.Topic = "your-another-topic";
            config.WaitNextConsume = false; // won't wait for the user and will consume in sequence
            config.ConsumerConfig.GroupId = "your-another-group";
            config.ConsumerConfig.BootstrapServers = "localhost:9092";
        });
    })
    .Build();

await host.RunAsync();
