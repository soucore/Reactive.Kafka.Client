using Confluent.Kafka;
using Reactive.Kafka;

namespace UsingAbstractClass.Consumers
{
    public class Consumer1 : ConsumerBase<Message>
    {
        private readonly ILogger<Consumer1> _logger;

        public Consumer1(ILogger<Consumer1> logger)
        {
            _logger = logger;
        }

        public override void OnConsumerConfiguration(IConsumer<string, string> consumer)
        {
            consumer.Subscribe("topic1");
        }

        public override Task Consume(object sender, KafkaMessage<Message> kafkaMessage, Commit commit)
        {
            _logger.LogInformation($"[Thread: {Environment.CurrentManagedThreadId}] {@kafkaMessage.Message}");
            
            return Task.CompletedTask;
        }
    }
}
