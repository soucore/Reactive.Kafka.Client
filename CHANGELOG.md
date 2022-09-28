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
