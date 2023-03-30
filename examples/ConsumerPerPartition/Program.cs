using Confluent.Kafka;
using ConsumerPerPartition;
using Newtonsoft.Json;
using Reactive.Kafka.Extensions;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddReactiveKafka((provider, configurator) =>
        {
            configurator.AddConsumerPerPartition<Consumer1, Message>("localhost:9092", (provider, cfg) =>
            {
                cfg.Topic = "your-topic";
                cfg.ConsumerConfig.GroupId = "your-group";
                cfg.ConsumerConfig.AutoOffsetReset = AutoOffsetReset.Latest;
                cfg.UseNewtonsoft(settings =>
                {
                    settings.MissingMemberHandling = MissingMemberHandling.Error;
                });
            });

            configurator.AddConsumerPerPartition<Consumer2, Message>("localhost:9092", (provider, cfg) =>
            {
                cfg.Topic = "your-another-topic";
                cfg.ConsumerConfig.GroupId = "your-another-group";
                cfg.UseSystemTextJson(options =>
                {
                    options.PropertyNameCaseInsensitive = true;
                });
            });
        });
    })
    .Build();

await host.RunConsumersAsync();
await host.RunAsync();
