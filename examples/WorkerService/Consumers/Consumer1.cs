using Confluent.Kafka;
using Reactive.Kafka;
using Reactive.Kafka.Interfaces;

namespace WorkerService.Consumers
{
    public class Consumer1 : IKafkaConsumer<Message>, IKafkaConsumerBuilder
    {
        private readonly ILogger<Consumer1> _logger;

        public Consumer1(ILogger<Consumer1> logger)
        {
            _logger = logger;
        }

        public void OnConsumerBuilder(ConsumerConfig config)
        {
            config.GroupId = "Grupo1";
        }

        public void OnConsumerConfiguration(IConsumer<string, string> consumer)
        {
            consumer.Subscribe("teste-topicc");
        }

        public void Consume(object sender, KafkaEventArgs<Message> @event)
        {
            _logger.LogInformation($"[Thread: {Environment.CurrentManagedThreadId}] {@event.Message}");
        }
    }
}
