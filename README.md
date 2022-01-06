.Net Client for Apache Kafka<sup>TM</sup>
=========================================

Features:

- **Abstract** and **simplify** integrations with confluent kafka
- Possibility to run **multi consumers** in the same application
- Ease to run a consumer per partition using threads

## Usage

```csharp
/// Program.cs
using Reactive.Kafka.Extensions;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<Worker>();
        services.AddReactiveKafkaConsumerPerPartition<Consumer1>("localhost:9092");
    })
    .Build();

await host.RunAsync();
```

```csharp
/// Consumer1.cs
using Confluent.Kafka;
using Reactive.Kafka;
using Reactive.Kafka.Errors;

namespace ConsumerPerPartition
{
    public class Consumer1 : ConsumerBase<string>
    {
        private readonly ILogger _logger;

        public Consumer1(ILogger<Consumer1> logger)
            => _logger = logger;

        public override void Consume(object sender, KafkaEventArgs<string> @event)
        {
            _logger.LogInformation($"[Thread: {Environment.CurrentManagedThreadId}] {@event.Message}");
        }

        public override void OnConsumerConfiguration(IConsumer<string, string> consumer)
        {
            consumer.Subscribe("your-topic");
        }
    }
}
```
