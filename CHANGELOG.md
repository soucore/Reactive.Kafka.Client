# 3.0.0
### Enhancements
- `OnConsume` and `OnConsumeError` hooks now have a `ConsumerContext` parameter that encapsulates all information about consumer and the processed message. As a shortcut, you can use all commit overloads directly on the ConsumerContext.
- Changed the way you choose the json serializer. Now you have 2 methods `UseNewtonsoft` and `UseSystemTextJson`, each one with equivalent configuration class as optional. Default is Newtonsoft. Property `RespectObjectContract` has been removed.
- Changed the way the library chooses between System.Convert or Json Serialization, now based on typecode of the generic type.

# 2.1.4
### Enhancements
- Configuration property to set `JsonSerializerOptions`.
```csharp
config.JsonSerializerOptions = new(JsonSerializerDefaults.Web);
```

# 2.1.3
### Enhancements
- Ability to switch between Newtonsoft or System.Text.Json serialization providers.
```csharp
config.SerializerProvider = SerializerProvider.SystemTextJson; // Newtonsoft is the default.
```

# 2.1.2
### Enhancements
- Implemented `WaitForConsumersShutdown` to wait for all consumers finish their jobs on a graceful shutdown (or timeout is reached: 1 minute).

# 2.1.1
### Enhancements

- Set the `SetPartitionsRevokedHandler` handle as default to turn off librdkafka's automatic partition assignment/revocation, preventing issues caused by rebalancing with auto-commit disabled.

# 1.2.3
### Enhancements

- Added `IServiceProvider` as a parameter in the configuration.

# 1.2.2
### Enhancements

- Added exception class for specific handling of serialization errors.
- Added `KafkaConfiguration` to configure options such as not awaitable workloads and serialization process respecting the object's contract.
- Added two new overloads for `AddReactiveKafkaConsumerPerPartition` and `AddReactiveKafkaConsumerPerQuantity`.
- Possibility to set topic name at DI startup, overriding OnConsumerConfiguration in consumer class is now optional.

# 1.2.1
### Fixes

- Fixed bug when consumer classes used interfaces.

# 1.2.0
### Enhancements

- Added consumer health check engine.

# 1.1.0
### Enhancements

- Added acoupled producer in each consumer.
- Added `OnProducerBuilder` in object lifecycle for the producer configuration.

### Fixes

- `OnConsumerError` was not firing.
