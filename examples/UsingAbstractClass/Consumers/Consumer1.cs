using Confluent.Kafka;
using Reactive.Kafka;

namespace UsingAbstractClass.Consumers
{
    public class Consumer1 : ConsumerBase<Message>
    {
        private readonly ILogger<Consumer1> _logger;

        public Consumer1(ILogger<Consumer1> logger)
            => _logger = logger;

        public override Task Consume(ConsumerMessage<Message> consumerMessage, Commit commit)
        {
            _logger.LogInformation("Message ==> {Message}", consumerMessage.Message);

            return Task.CompletedTask;
        }

        public override void OnConsumerConfiguration(IConsumer<string, string> consumer)
        {
            consumer.Subscribe("your-topic");
        }
    }
}
