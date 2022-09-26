Reactive .Net Client for Apache Kafka<sup>TM</sup>
=========================================

[![latest version](https://img.shields.io/nuget/v/Reactive.Kafka.Client)](https://www.nuget.org/packages/Reactive.Kafka.Client/)
[![build status](https://img.shields.io/appveyor/build/RFPAlves/reactive-kafka-client/main)](https://ci.appveyor.com/project/RFPAlves/reactive-kafka-client/branch/main)
[![test results](https://img.shields.io/appveyor/tests/RFPAlves/reactive-kafka-client/main)](https://ci.appveyor.com/project/RFPAlves/reactive-kafka-client/branch/main/tests)

Features:

- **Abstract** and **simplify** integrations with confluent kafka.
- Possibility to run **multi consumers** in the same application.
- Ease to run a **consumer per partition** using threads.
- Message **deserialization** to the desired object effortless.
- Specific method for the correct treatment of **error messages**.

## Installation
To install Reactive.Kafka.Client from within Visual Studio, search for Reactive.Kafka.Client in the NuGet Package Manager UI, or run the following command in the Package Manager Console:

```
Install-Package Reactive.Kafka.Client -Version 2.1.1
```

To add a reference to a dotnet core project, execute the following at the command line:

```
dotnet add package -v 2.1.1 Reactive.Kafka.Client
```

## Message lifecycle

A message has a lifecycle that starts whenever a new message is obtained from topic. Your application can use lifecycle hook methods for the treatment or enrichment of the message.

### Responding to lifecycle events

Respond to events in the lifecycle of a message by overriding one or more of the lifecycle hook methods. The hooks give you the opportunity to act on a message before its use in your business logic.

```csharp
public class MyConsumer : ConsumerBase<Message>
{
    public override void OnConsumerConfiguration(ConsumerConfig configuration)
    {
        // your consumer configuration here.
    }

    public override string OnBeforeSerialization(string rawMessage)
    {
        // your treatment here.
    }
    
    public override Message OnAfterSerialization(Message message)
    {
        // your enrichment here.
    }
    
    public override Task OnConsume(ConsumerMessage<Message> consumerMessage, Commit commit)
    {
        // your business logic here.
        return Task.CompletedTask;
    }
}
```

Only `OnConsume` is required. The others are not required and you implement just the ones you need.

### Lifecycle event sequence

| Hook method | Purpose | Timing | Required |
|--------------|--------------|--------------|--------------|
| OnConsumerConfiguration | | Called once, for each consumer instance, during the consumer setup process. | No |
| OnProducerConfiguration | Producer instance for message forwarding. | Called once, for each consumer instance, during the producer setup process. | No |
| OnConsumerBuilder | | Called once, for each consumer instance, before the kafka consumer is built. | No |
| OnReady | | Called once, for each consumer instance, after the kafka consumer is built. | No |
| OnBeforeSerialization | Treatment of the message. | Called after topic message consumption and before `OnAfterSerialization`. | No |
| OnAfterSerialization | Enrichment of the message. | Called after the serialization process, may not occur if serialization fails. | No |
| OnConsume | Business logic. | Called immediately after `OnAfterSerialization` for each message. | Yes |
| OnConsumeError | | Called when serialization process fails. | No |

## Concept
![Concept Image](docs/concept.png)

## Usage

- Exclusive thread per consumer.
- Possibility to inject anything from DI (Dependency Injection) in your consumer.

Check out our examples for a full demonstration. 😉

### Simplest Kafka Consumer ever

With few lines you have a Kafka Consumer taking advantage of each partition.

```csharp
// ConsumerExample.cs
public class ConsumerExample : ConsumerBase<string>
{
    public override Task OnConsume(ConsumerMessage<string> consumerMessage, Commit commit)
    {
        Console.WriteLine(consumerMessage.Message);
        return Task.CompletedTask;
    }
}
```

```csharp
// Program.cs
using Reactive.Kafka.Extensions;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddReactiveKafka((provider, configurator) => {
            configurator.AddConsumerPerPartition<ConsumerExample, string>("localhost:9092", "your-topic", "your-group");
        });
    })
    .Build();

await host.RunConsumersAsync();
await host.RunAsync();
```

### AddReactiveKafkaConsumerPerPartition

Creates a consumer per partition of a given topic.

```csharp
// Message.cs
public class Message
{
    public int Id { get; set; }
    public string Name { get; set; }
}
```

```csharp
// ConsumerExample.cs
public class ConsumerExample : ConsumerBase<Message>
{
    public override void OnProducerConfiguration(ProducerConfig configuration)
    {
        configuration.BootstrapServers = "localhost:9092";
        configuration.Acks = Acks.None;
    }

    public override async Task OnConsume(ConsumerMessage<Message> consumerMessage, Commit commit)
    {       
        if (consumerMessage.Id == 0) {
            await ProducerAsync("DeadLetterTopic", consumerMessage.Message.ToString());
            return;
        }
        
        Console.WriteLine(consumerMessage.Message);
    }
}
```

```csharp
// Program.cs
using Reactive.Kafka.Extensions;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddReactiveKafka((provider, configurator) => {
            configurator.AddConsumerPerPartition<ConsumerExample, Message>("localhost:9092", (provider, config) => {
                // json messages that don't agree with the object will throw an exception.
                // You can catch and handle it in the OnConsumeError method.
                config.RespectObjectContract = true; 

                config.Topic = "your-topic";
                config.ConsumerConfig.GroupId = "your-group";
                config.ConsumerConfig.AutoOffsetReset = AutoOffsetReset.Latest;
                config.ConsumerConfig.AutoCommitIntervalMs = 0;
                config.ConsumerConfig.EnableAutoCommit = false;               
            });
        });
    })
    .Build();

await host.RunConsumersAsync();
await host.RunAsync();
```

### AddReactiveKafkaConsumerPerQuantity

Creates a specified number of consumer in a given topic.

```csharp
// ConsumerExample.cs
public class ConsumerExample : ConsumerBase<string>
{   
    public override string OnBeforeSerialization(string rawMessage)
    {
        string newMessage = Regex.Replace(rawMessage, @"\D", "");
        return newMessage;
    }

    public override Task OnConsume(ConsumerMessage<string> consumerMessage, Commit commit)
    {
        Console.WriteLine(consumerMessage.Message);
        return Task.CompletedTask;
    }
}
```

```csharp
// Program.cs
using Reactive.Kafka.Extensions;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddReactiveKafka((provider, configurator) => {
            configurator.AddConsumerPerQuantity<ConsumerExample, string>("localhost:9092", quantity: 2, "your-topic", "your-group");
        });
    })
    .Build();

await host.RunConsumersAsync();
await host.RunAsync();
```
