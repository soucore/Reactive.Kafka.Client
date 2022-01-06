using Confluent.Kafka;
using Reactive.Kafka;
using Reactive.Kafka.Errors;
using Reactive.Kafka.Interfaces;

namespace WorkerService.Consumers
{
    public class Consumer5 : IKafkaConsumer<string>, IKafkaConsumerBuilder, IKafkaConsumerError
    {
        public Consumer5(ILogger<Consumer5> logger)
        {
            Logger = logger;
        }

        public ILogger<Consumer5> Logger { get; }

        public void Consume(object sender, KafkaEventArgs<string> @event)
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
            builder.GroupId = "Group100";
        }
    }
}
