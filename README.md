Reactive .Net Client for Apache Kafka<sup>TM</sup>
=========================================

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
Install-Package Reactive.Kafka.Client -Version 1.0.0
```

To add a reference to a dotnet core project, execute the following at the command line:

```
dotnet add package -v 1.0.0 Reactive.Kafka.Client
```

## Message lifecycle

A message has a lifecycle that starts whenever a new message is obtained from topic. Your application can use lifecycle hook methods for the treatment or enrichment of the message.

### Responding to lifecycle events

Respond to events in the lifecycle of a message by overriding one or more of the lifecycle hook methods. The hooks give you the opportunity to act on a message before its use in your business logic.

```csharp
public class MyConsumer : ConsumerBase<Message>
{
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
    
    public override void OnConsumerBuilder(ConsumerConfig builder)
    {
        // your consumer configuration here.
    }
}
```

`OnConsume` and `OnConsumerBuilder` are required. The others are not required and you implement just the ones you need.

### Lifecycle event sequence

| Hook method | Purpose | Timing | Required |
|------------------------|------------|--------|--------|
| OnConsumerBuilder      |                            | Called once, for each consumer instance, before confluent kafka consumer.  | No  |
| OnConsumerConfigration |                            | Called once, after confluent kafka consumer has been built.                | Yes |
| OnBeforeSerialization  | Treatment of the message.  | Called after message consume from topic and before `OnAfterSerialization`. | No  |
| OnAfterSerialization   | Enrichment of the message. | Called after serialization process, may not occur if serialization fails.  | No  |
| OnConsume              | Business logic.            | Called immediately after `OnAfterSerialization` for each message.          | Yes |
| OnConsumeError         |                            | Called when serialization process fails.                                   | No  |

## Concept
![Concept Image](docs/concept.png)
