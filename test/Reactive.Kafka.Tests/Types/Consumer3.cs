using Confluent.Kafka;
using System.Threading.Tasks;

namespace Reactive.Kafka.Tests.Types
{
    public class Consumer3 : ConsumerBase<string>
    {
        public override Task OnConsume(ConsumerMessage<string> consumerMessage, Commit commit)
        {
            return Task.CompletedTask;
        }

        public override void OnConsumerConfiguration(IConsumer<string, string> consumer)
        {
            consumer.Subscribe("topic1");
        }

        public override void OnConsumerBuilder(ConsumerConfig builder)
        {
            builder.BootstrapServers = "localhost:9092";
            builder.GroupId = "Group";

            base.OnConsumerBuilder(builder);
        }

        public override void OnProducerBuilder(ProducerConfig builder)
        {
            builder.BootstrapServers = "localhost:9092";
            builder.Acks = Acks.None;

            base.OnProducerBuilder(builder);
        }
    }
}
