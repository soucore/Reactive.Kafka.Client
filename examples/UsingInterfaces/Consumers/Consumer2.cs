using Confluent.Kafka;
using Reactive.Kafka;
using Reactive.Kafka.Interfaces;

namespace UsingInterfaces.Consumers
{
    public class Consumer2 : IKafkaConsumer<string>
    {
        private readonly ILogger<Consumer2> _logger;

        public Consumer2(ILogger<Consumer2> logger)
        {
            _logger = logger;
        }

        public void OnConsumerConfiguration(IConsumer<string, string> consumer)
        {
            consumer.Subscribe("topic2");
        }

        public void Consume(object sender, KafkaEventArgs<string> @event)
        {
            // log thread for analysis purpose
            _logger.LogInformation($"[Thread: {Environment.CurrentManagedThreadId}] {@event.Message}");
        }
    }
}
