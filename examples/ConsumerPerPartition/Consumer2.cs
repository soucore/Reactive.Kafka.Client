using Confluent.Kafka;
using Reactive.Kafka;

namespace ConsumerPerPartition
{
    public class Consumer2 : ConsumerBase<Message>
    {
        private readonly ILogger<Consumer2> _logger;

        public Consumer2(ILogger<Consumer2> logger)
        {
            _logger = logger;
        }

        public override async Task OnConsume(ConsumerMessage<Message> consumerMessage, Commit commit)
        {
            _logger.LogInformation("{Message}", consumerMessage.Message.FirstName);
            await Task.Delay(500);
            _logger.LogInformation("Good job!!");
        }

        public override void OnConsumerBuilder(ConsumerBuilder<string, string> builder)
        {
            builder.SetErrorHandler((consumer, error) => _logger.LogError("{ErrorMessage}", error.Reason));
        }

        public override void OnProducerConfiguration(ProducerConfig configuration)
        {
            configuration.BootstrapServers = "localhost:9092";
        }
    }
}
