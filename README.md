Reactive .Net Client for Apache Kafka<sup>TM</sup>
=========================================

Features:

- **Abstract** and **simplify** integrations with confluent kafka
- Possibility to run **multi consumers** in the same application
- Ease to run a **consumer per partition** using threads
- Message **deserialization** to the desired object effortless
- Specific method for the correct treatment of **error messages**

## Usage

Check out our examples for a full demonstration of Reactive Kafka Consumer features.

### Basic Consumer using interfaces

```csharp
/// Program.cs
using Confluent.Kafka;
using Reactive.Kafka.Extensions;
using UsingInterfaces;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<Worker>();
        services.AddReactiveKafkaConsumer(config =>
        {
            config.BootstrapServers = "localhost:9092";
            config.GroupId = "YourGroup";
            config.AutoOffsetReset = AutoOffsetReset.Earliest;
        });
    })
    .Build();

await host.RunAsync();
```

```csharp
/// Consumer1.cs
using Confluent.Kafka;
using Reactive.Kafka;
using Reactive.Kafka.Errors;
using Reactive.Kafka.Interfaces;

namespace UsingInterfaces.Consumers
{
    public class Consumer1 : IKafkaConsumer<string>, IKafkaConsumerError
    {
        private readonly ILogger<Consumer1> _logger;

        // You can inject anything from DI
        public Consumer1(ILogger<Consumer1> logger)
            => _logger = logger;

        public void OnConsumerConfiguration(IConsumer<string, string> consumer)
        {
            consumer.Subscribe("topic1");
        }

        public void Consume(object sender, KafkaEventArgs<string> @event)
        {
            _logger.LogInformation($"[Thread: {Environment.CurrentManagedThreadId}] {@event.Message}");
        }

        public void ConsumeError(object sender, KafkaConsumerError consumerError)
        {
            _logger.LogError($"[Thread: {Environment.CurrentManagedThreadId}] {consumerError.Exception.Message}");
        }
    }
}
```

### Basic Consumer using abstract class

```csharp
/// Program.cs
using Reactive.Kafka.Extensions;
using UsingAbstractClass;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<Worker>();
        services.AddReactiveKafkaConsumer("localhost:9092");
    })
    .Build();

await host.RunAsync();
```

```csharp
using Confluent.Kafka;
using Reactive.Kafka;

namespace UsingAbstractClass.Consumers
{
    public class Consumer1 : ConsumerBase<Message>
    {
        private readonly ILogger<Consumer1> _logger;

        public Consumer1(ILogger<Consumer1> logger)
            => _logger = logger;

        public override void Consume(object sender, KafkaEventArgs<Message> @event)
        {
            _logger.LogInformation($"[Thread: {Environment.CurrentManagedThreadId}] {@event.Message}");
        }

        public override void OnConsumerConfiguration(IConsumer<string, string> consumer)
        {
            consumer.Subscribe("YourTopic");
        }
    }

    public record Message(int Id, string Name);
}
```

### Basic Consumer Per Partition

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
