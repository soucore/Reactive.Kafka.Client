using Confluent.Kafka;
using Reactive.Kafka;

namespace UsingAbstractClass.Consumers
{
    public class Consumer2 : ConsumerBase<string>
    {
        private readonly ILogger<Consumer2> _logger;

        public Consumer2(ILogger<Consumer2> logger)
        {
            _logger = logger;
        }

        public override void OnConsumerBuilder(ConsumerConfig builder)
        {
            builder.GroupId = "Group2";
            builder.AutoOffsetReset = AutoOffsetReset.Latest;

            base.OnConsumerBuilder(builder);
        }

        public override void OnConsumerConfiguration(IConsumer<string, string> consumer)
        {
            consumer.Subscribe("topic2");
        }

        public override Task Consume(object sender, KafkaMessage<string> kafkaMessage, Commit commit)
        {
            _logger.LogInformation($"[Thread: {Environment.CurrentManagedThreadId}] {kafkaMessage.Message}");

            return Task.CompletedTask;
        }
    }
}
