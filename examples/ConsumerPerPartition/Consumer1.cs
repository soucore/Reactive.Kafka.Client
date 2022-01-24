using Confluent.Kafka;
using Reactive.Kafka;

namespace ConsumerPerPartition
{
    internal class Consumer1 : ConsumerBase<Message>
    {
        public override Task Consume(ConsumerMessage<Message> consumerMessage, Commit commit)
        {
            return Task.CompletedTask;
        }

        public override void OnConsumerConfiguration(IConsumer<string, string> consumer)
        {
            consumer.Subscribe("your-topic");
        }
    }
}
