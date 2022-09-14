using System.Threading.Tasks;

namespace Reactive.Kafka.Tests.Types;

public class Consumer4OnInit : ConsumerBase<string>
{
    public bool IsInit { get; set; }
    public override Task OnConsume(ConsumerMessage<string> consumerMessage, Commit commit)
    {
        return Task.CompletedTask;
    }

    public override void OnInit()
    {
        IsInit = true;
    }
}
