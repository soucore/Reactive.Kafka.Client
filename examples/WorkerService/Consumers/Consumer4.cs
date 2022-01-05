using Confluent.Kafka;
using Reactive.Kafka;

namespace WorkerService.Consumers
{
    public class Consumer4 : ConsumerBase<string>
    {
        public Consumer4(ILogger<Consumer4> logger)
        {
            Logger = logger;
        }

        public ILogger<Consumer4> Logger { get; }

        public override void Consume(object sender, KafkaEventArgs<string> @event)
        {
            Logger.LogInformation($"[Thread: {Environment.CurrentManagedThreadId}] {@event.Message}");
        }

        public override void OnConsumerConfiguration(IConsumer<string, string> consumer)
        {
            consumer.Subscribe("topic900");
        }

        public override void OnConsumerBuilder(ConsumerConfig builder)
        {
            builder.GroupId = "Group100";
            base.OnConsumerBuilder(builder);
        }
    }
}
