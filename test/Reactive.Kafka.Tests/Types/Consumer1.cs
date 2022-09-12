using System.Threading.Tasks;

namespace Reactive.Kafka.Tests.Types;

public class Consumer1 : ConsumerBase<string>
{
    public override Task OnConsume(ConsumerMessage<string> consumerMessage, Commit commit)
    {
        return Task.CompletedTask;
    }
}
