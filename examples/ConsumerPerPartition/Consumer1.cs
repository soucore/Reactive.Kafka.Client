using Reactive.Kafka;

namespace ConsumerPerPartition
{
    public class Consumer1 : ConsumerBase<string>
    {
        private readonly ILogger<Consumer1> _logger;

        public Consumer1(ILogger<Consumer1> logger)
        {
            _logger = logger;
        }

        public override Task OnConsume(ConsumerMessage<string> consumerMessage, ConsumerContext context)
        {
            _logger.LogInformation("{Message}", consumerMessage.Value);
            return Task.CompletedTask;
        }
    }
}
