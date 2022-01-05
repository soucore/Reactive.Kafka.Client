using Confluent.Kafka;
using Reactive.Kafka;
using Reactive.Kafka.Errors;

namespace UsingAbstractClass.Consumers
{
    public class Consumer2 : ConsumerBase<string>
    {
        private readonly ILogger<Consumer2> _logger;

        public Consumer2(ILogger<Consumer2> logger)
        {
            _logger = logger;
        }

        public override void Consume(object sender, KafkaEventArgs<string> @event)
        {
            // log thread for analysis purpose
            _logger.LogInformation($"[Thread: {Environment.CurrentManagedThreadId}] {@event.Message}");
        }

        public override void ConsumeError(object sender, KafkaConsumerError consumerError)
        {
            _logger.LogError($"[Thread: {Environment.CurrentManagedThreadId}] {consumerError.Exception.Message}");
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
    }
}
