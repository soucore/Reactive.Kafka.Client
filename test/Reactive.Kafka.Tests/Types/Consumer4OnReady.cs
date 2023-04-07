using System.Threading;
using System.Threading.Tasks;

namespace Reactive.Kafka.Tests.Types;

public class Consumer4OnReady : ConsumerBase<string>
{
    public bool IsInit { get; set; }

    public override Task OnConsume(ConsumerMessage<string> consumerMessage, ConsumerContext context, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public override void OnReady()
    {
        IsInit = true;
    }
}
