using Confluent.Kafka;
using Reactive.Kafka.Extensions;
using WorkerService;
using WorkerService.Consumers;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<Worker>();

        //services.AddReactiveKafkaConsumer(config =>
        //{
        //    config.BootstrapServers = "localhost:9092";
        //    config.GroupId = "Group200";
        //});

        services.AddReactiveKafkaConsumerPerPartition<Consumer6>("localhost:9092");
    })
    .Build();

await host.RunAsync();
