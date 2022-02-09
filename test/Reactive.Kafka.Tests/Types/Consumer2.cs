using Confluent.Kafka;
using System.Threading.Tasks;

namespace Reactive.Kafka.Tests.Types
{
    public class Consumer2 : ConsumerBase<string>
    {
        public override Task OnConsume(ConsumerMessage<string> consumerMessage, Commit commit)
        {
            return Task.CompletedTask;
        }

        public override void OnConsumerConfiguration(IConsumer<string, string> consumer)
        {
            consumer.Subscribe("test-topic");
        }
    }
}
