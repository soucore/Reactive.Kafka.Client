using System.Threading.Tasks;

namespace Reactive.Kafka.Tests.Types;

public class Consumer3 : ConsumerBase<string>
{
    public override Task OnConsume(ConsumerMessage<string> consumerMessage, ConsumerContext context)
    {
        return Task.CompletedTask;
    }

    public override void OnConsumerConfiguration(ConsumerConfig configuration)
    {
        configuration.BootstrapServers = "localhost:9092";
        configuration.GroupId = "Group";
    }

    public override void OnProducerConfiguration(ProducerConfig configuration)
    {
        configuration.BootstrapServers = "localhost:9092";
        configuration.Acks = Acks.None;
    }

    public override string OnAfterSerialization(string message)
    {
        return base.OnAfterSerialization(message);
    }

    public override string OnBeforeSerialization(string rawMessage)
    {
        return base.OnBeforeSerialization(rawMessage);
    }
}
