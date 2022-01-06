using Confluent.Kafka;
using Reactive.Kafka;
using Reactive.Kafka.Interfaces;

namespace WorkerService.Consumers
{
    public class Consumer3 : IKafkaConsumer<Message>, IKafkaConsumerBuilder
    {
        public Consumer3(ILogger<Consumer3> logger)
        {
            Logger = logger;
        }

        public ILogger<Consumer3> Logger { get; }

        public void OnConsumerBuilder(ConsumerConfig builder)
        {
            builder.GroupId = "Group100";
        }

        public void OnConsumerConfiguration(IConsumer<string, string> consumer)
        {
            consumer.Subscribe("topic900");
        }

        public void Consume(object sender, KafkaEventArgs<Message> @event)
        {
            Logger.LogInformation($"[Thread: {Environment.CurrentManagedThreadId}] {@event.Message}");
        }
    }
}
