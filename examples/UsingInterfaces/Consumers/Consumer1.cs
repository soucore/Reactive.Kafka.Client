using Confluent.Kafka;
using Reactive.Kafka;
using Reactive.Kafka.Errors;
using Reactive.Kafka.Interfaces;

namespace UsingInterfaces.Consumers
{
    public class Consumer1 : IKafkaConsumer<string>, IKafkaConsumerBuilder, IKafkaConsumerError
    {
        private readonly ILogger<Consumer1> _logger;

        // You can inject anything from DI
        public Consumer1(ILogger<Consumer1> logger)
        {
            _logger = logger;
        }

        public void OnConsumerBuilder(ConsumerConfig builder)
        {
            builder.GroupId = "Group1";
            builder.AutoOffsetReset = AutoOffsetReset.Latest;
        }

        public void OnConsumerConfiguration(IConsumer<string, string> consumer)
        {
            consumer.Subscribe("topic1");
        }

        public void Consume(object sender, KafkaEventArgs<string> @event)
        {
            // log thread for analysis purpose
            _logger.LogInformation($"[Thread: {Environment.CurrentManagedThreadId}] {@event.Message}");
        }

        public void ConsumeError(object sender, KafkaConsumerError consumerError)
        {
            _logger.LogError($"[Thread: {Environment.CurrentManagedThreadId}] {consumerError.Exception.Message}");
        }
    }
}
