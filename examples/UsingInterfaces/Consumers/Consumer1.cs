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

        public Task Consume(object sender, KafkaMessage<string> kafkaMessage, Commit commit)
        {
            _logger.LogInformation($"[Thread: {Environment.CurrentManagedThreadId}] {kafkaMessage.Message}");
            
            return Task.CompletedTask;
        }

        public void OnConsumerConfiguration(IConsumer<string, string> consumer)
        {
            consumer.Subscribe("topic1");
        }

        public Task ConsumeError(object sender, KafkaConsumerError consumerError, Commit commit)
        {
            _logger.LogInformation($"[Thread[Error Exception]: {Environment.CurrentManagedThreadId}] {consumerError.KafkaMessage}");
            
            return Task.CompletedTask;
        }
    }
}
