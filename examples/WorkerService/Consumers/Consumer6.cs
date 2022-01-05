using Confluent.Kafka;
using Reactive.Kafka;
using Reactive.Kafka.Errors;
using Reactive.Kafka.Interfaces;

namespace WorkerService.Consumers
{
    public class Consumer6 : IKafkaConsumer<Message>, IKafkaConsumerBuilder, IKafkaConsumerError
    {
        public Consumer6(ILogger<Consumer6> logger)
        {
            Logger = logger;
        }

        public ILogger<Consumer6> Logger { get; }

        public void Consume(object sender, KafkaEventArgs<Message> @event)
        {
            Logger.LogInformation($"[Thread: {Environment.CurrentManagedThreadId}] {@event.Message}");
        }

        public void ConsumeError(object sender, KafkaConsumerError consumerError)
        {
            Logger.LogInformation($"[Thread: {Environment.CurrentManagedThreadId}][Error] {consumerError.KafkaMessage}");
        }

        public void OnConsumerConfiguration(IConsumer<string, string> consumer)
        {
            consumer.Subscribe("topic900");
        }

        public void OnConsumerBuilder(ConsumerConfig builder)
        {
            builder.GroupId = "Group1";
        }
    }
}
