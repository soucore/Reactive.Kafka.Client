using Confluent.Kafka;
using Reactive.Kafka;
using Reactive.Kafka.Errors;

namespace ConsumerPerPartition
{
    public class Consumer1 : ConsumerBase<string>
    {
        private readonly ILogger _logger;

        public Consumer1(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<Consumer1>();
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

        public override void OnConsumerConfiguration(IConsumer<string, string> consumer)
        {
            consumer.Subscribe("topic2");
        }

        public override void OnConsumerBuilder(ConsumerConfig builder)
        {
            builder.GroupId = "Group2";
            builder.AutoOffsetReset = AutoOffsetReset.Latest;

            base.OnConsumerBuilder(builder);
        }
    }
}
