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

        public Task OnConsume(ConsumerMessage<string> consumerMessage, Commit commit)
        {
            _logger.LogInformation("{Message}", consumerMessage.Message);
            return Task.CompletedTask;
        }
    }
}
