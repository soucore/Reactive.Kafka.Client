using Confluent.Kafka;
using Reactive.Kafka;
using Reactive.Kafka.Interfaces;

namespace WorkerService.Consumers
{
    public class Consumer1 : IKafkaConsumer<Message>, IKafkaConsumerBuilder
    {
        public void Consume(object sender, KafkaEventArgs<Message> @event)
        {
            Console.WriteLine($"[{Environment.CurrentManagedThreadId}] [Key= {@event.Key}] [Message= {@event.Message}]");
        }

        public void OnConsumerConfiguration(IConsumer<string, string> consumer)
        {
            consumer.Subscribe("teste-topic");
        }

        public void OnConsumerBuilder(ConsumerConfig config)
        {
            config.GroupId = "Grupo1";
        }
    }
}
