using Confluent.Kafka;
using System.Threading.Tasks;

namespace Reactive.Kafka.Tests.Types
{
    public class Consumer1 : ConsumerBase<string>
    {
        public override async Task Consume(ConsumerMessage<string> consumerMessage, Commit commit) { }
        public override void OnConsumerConfiguration(IConsumer<string, string> consumer) { }
    }
}
