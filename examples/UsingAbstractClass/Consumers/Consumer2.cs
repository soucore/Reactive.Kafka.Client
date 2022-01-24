using Confluent.Kafka;
using Reactive.Kafka;

namespace UsingAbstractClass.Consumers
{
    public class Consumer2 : ConsumerBase<string>
    {
        private readonly ILogger<Consumer2> _logger;

        public Consumer2(ILogger<Consumer2> logger)
            => _logger = logger;

        public override Task Consume(ConsumerMessage<string> consumerMessage, Commit commit)
        {
            _logger.LogInformation("Message ==> {Message}", consumerMessage.Message);

            return Task.CompletedTask;
        }

        public override void OnConsumerBuilder(ConsumerConfig builder)
        {
            builder.GroupId = "YourGroup";
            builder.AutoOffsetReset = AutoOffsetReset.Latest;

            base.OnConsumerBuilder(builder);
        }

        public override void OnConsumerConfiguration(IConsumer<string, string> consumer)
        {
            consumer.Subscribe("your-topic");
        }
    }
}
