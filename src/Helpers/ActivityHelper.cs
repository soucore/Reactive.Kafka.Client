using System.Diagnostics;

namespace Reactive.Kafka.Helpers;

internal static class ActivityHelper
{
    public static readonly ActivitySource source = new("Reactive.Kafka.Client.Diagnostics");

    public static Activity CreateConsumerActivity(ConsumeResult<string, string> consumeResult, string memberId)
    {
        var activity = source.StartActivity($"{consumeResult.Topic} process", ActivityKind.Consumer);

        if (activity?.IsAllDataRequested ?? false)
        {
            activity?.SetTag("kafka.topic", consumeResult.Topic);
            activity?.SetTag("kafka.partition", consumeResult.Partition.Value.ToString());
            activity?.SetTag("kafka.offset", consumeResult.Offset.Value.ToString());
            activity?.SetTag("kafka.consumer.id", memberId);
        }

        return activity;
    }

    public static Activity CreateProducerActivity(string topic)
    {
        var activity = source.StartActivity($"{topic} publish", ActivityKind.Producer);

        if (activity?.IsAllDataRequested ?? false)
        {
            activity?.SetTag("kafka.topic", topic);
        }

        return activity;
    }
}
