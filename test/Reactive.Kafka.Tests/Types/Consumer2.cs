using System.Threading;
using System.Threading.Tasks;

namespace Reactive.Kafka.Tests.Types;

public class Consumer2 : ConsumerBase<string>
{
    public override Task OnConsume(ConsumerMessage<string> consumerMessage, ConsumerContext context, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
