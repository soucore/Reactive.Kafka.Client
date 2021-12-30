using Confluent.Kafka;
using Reactive.Kafka;
using Reactive.Kafka.Errors;
using Reactive.Kafka.Interfaces;

namespace WorkerService.Consumers
{
    public class Consumer2 : IKafkaConsumer<Message>, IKafkaConsumerBuilder, IKafkaConsumerError
    {
        private readonly ILogger<Consumer2> _logger;

        public Consumer2(ILogger<Consumer2> logger)
        {
            _logger = logger;
        }

        public void OnConsumerBuilder(ConsumerConfig config)
        {
            config.GroupId = "Group2";
            config.AutoOffsetReset = AutoOffsetReset.Earliest;
        }

        public void OnConsumerConfiguration(IConsumer<string, string> consumer)
        {
            consumer.Subscribe("teste-topic");
        }

        public void Consume(object sender, KafkaEventArgs<Message> @event)
        {
            _logger.LogInformation($"[Thread: {Environment.CurrentManagedThreadId}] {@event.Message}");
        }

        public void ConsumeError(object sender, KafkaConsumerError consumerError)
        {
            _logger.LogError($"[Thread: {Environment.CurrentManagedThreadId}] {consumerError.Exception.Message}");
        }
    }
}
