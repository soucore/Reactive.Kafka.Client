using Confluent.Kafka;
using Reactive.Kafka;
using Reactive.Kafka.Errors;
using Reactive.Kafka.Interfaces;

namespace WorkerService.Consumers
{
    public class Consumer2 : IKafkaConsumer<Message>, IKafkaConsumerBuilder, IKafkaConsumerError
    {

        public void Consume(object sender, KafkaEventArgs<Message> @event)
        {
            Console.WriteLine($"[{Environment.CurrentManagedThreadId}] [Key= {@event.Key}] [Message= {@event.Message}]");
        }

        public void ConsumeError(object sender, KafkaConsumerError consumerError)
        {
            Console.WriteLine($"[{Environment.CurrentManagedThreadId}][Error][Consumer2] {consumerError.Exception.Message}");
        }

        public void OnConsumerBuilder(ConsumerConfig config)
        {
            config.GroupId = "Group2";
            config.AutoOffsetReset = AutoOffsetReset.Earliest;
        }

        public void OnConsumerConfiguration(IConsumer<string, string> consumer)
        {
            consumer.Subscribe("teste-topic");
        }
    }
}
