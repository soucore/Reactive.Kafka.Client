using Reactive.Kafka;

namespace ConsumerPerPartition
{
    public class Consumer2 : ConsumerBase<string>
    {
        private readonly ILogger<Consumer2> _logger;

        public Consumer2(ILogger<Consumer2> logger)
        {
            _logger = logger;
        }

        public override async Task OnConsume(ConsumerMessage<string> consumerMessage, Commit commit)
        {
            _logger.LogInformation("{Message}", consumerMessage.Message);
            await Task.Delay(500);
            _logger.LogInformation("Good job!");
        }
    }
}
