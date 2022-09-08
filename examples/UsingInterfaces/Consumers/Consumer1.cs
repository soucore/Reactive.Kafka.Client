using Confluent.Kafka;
using Reactive.Kafka;
using Reactive.Kafka.Errors;
using Reactive.Kafka.Interfaces;

namespace UsingInterfaces.Consumers
{
    public class Consumer1 : IKafkaConsumer<string>, IKafkaConsumerError, IKafkaConsumerConfiguration
    {
        private readonly ILogger<Consumer1> _logger;

        public Consumer1(ILogger<Consumer1> logger)
        {
            _logger = logger;
        }

        public Task OnConsume(ConsumerMessage<string> consumerMessage, Commit commit)
        {
            _logger.LogInformation("{Message}", consumerMessage.Message);
            return Task.CompletedTask;
        }

        public Task OnConsumeError(KafkaConsumerError consumerError, Commit commit)
        {
            _logger.LogError("Ops! Something is wrong.");
            return Task.CompletedTask;
        }

        public void OnConsumerConfiguration(ConsumerConfig configuration)
        {
            configuration.GroupId = "your-group";
            configuration.AutoOffsetReset = AutoOffsetReset.Latest;
        }
    }
}
