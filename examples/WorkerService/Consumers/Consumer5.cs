using Confluent.Kafka;
using Reactive.Kafka;
using Reactive.Kafka.Errors;

namespace WorkerService.Consumers
{
    public class Consumer5 : ConsumerBase<string>
    {
        public Consumer5(ILogger<Consumer5> logger)
        {
            Logger = logger;
        }

        public ILogger<Consumer5> Logger { get; }

        public override void Consume(object sender, KafkaEventArgs<string> @event)
        {
            Logger.LogInformation($"[Thread: {Environment.CurrentManagedThreadId}] {@event.Message}");
        }

        public override void ConsumeError(object sender, KafkaConsumerError consumerError)
        {
            Logger.LogInformation($"[Thread: {Environment.CurrentManagedThreadId}][Error] {consumerError.KafkaMessage}");
        }

        public override void OnConsumerConfiguration(IConsumer<string, string> consumer)
        {
            consumer.Subscribe("topic100");
        }

        public override void OnConsumerBuilder(ConsumerConfig builder)
        {
            builder.GroupId = "Group100";
            base.OnConsumerBuilder(builder);
        }
    }
}
