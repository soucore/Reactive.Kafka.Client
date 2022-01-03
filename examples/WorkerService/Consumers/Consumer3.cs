using Confluent.Kafka;
using Reactive.Kafka;

namespace WorkerService.Consumers
{
    public class Consumer3 : ConsumerBase<Message>
    {
        public Consumer3(ILogger<Consumer3> logger)
        {
            Logger = logger;
        }

        public ILogger<Consumer3> Logger { get; }

        public override void OnConsumerBuilder(ConsumerConfig builder)
        {
            builder.GroupId = "Group100";
            base.OnConsumerBuilder(builder);
        }

        public override void OnConsumerConfiguration(IConsumer<string, string> consumer)
        {
            consumer.Subscribe("topic100");
        }

        public override void Consume(object sender, KafkaEventArgs<Message> @event)
        {
            Logger.LogInformation($"[Thread: {Environment.CurrentManagedThreadId}] {@event.Message}");
        }
    }
}
