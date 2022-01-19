using Confluent.Kafka;
using Reactive.Kafka;

namespace ConsumerPerPartition
{
    internal class Consumer1 : ConsumerBase<string>
    {
        public override Task Consume(object sender, KafkaMessage<string> kafkaMessage, Commit commit)
        {
            return Task.CompletedTask;
        }

        public override void OnConsumerConfiguration(IConsumer<string, string> consumer)
        {
            consumer.Subscribe("novo-topico");
        }
    }
}
