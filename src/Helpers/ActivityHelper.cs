namespace Reactive.Kafka.Helpers;

internal static class ActivityHelper
{
    #region W3C Trace Context standard
    private static readonly string TraceParentHeader = "traceparent";
    private static readonly string TraceStateHeader = "tracestate";
    #endregion

    #region Tags
    private static readonly string KafkaMessageTopic = "kafka.message.topic";
    private static readonly string KafkaMessagePartition = "kafka.message.partition";
    private static readonly string KafkaMessageOffset = "kafka.message.offset";
    private static readonly string KafkaMessageConsumerId = "kafka.message.consumer.id";
    private static readonly string KafkaMessageKey = "kafka.message.key";
    #endregion

    private static readonly ActivitySource source = new("Reactive.Kafka.Client.Diagnostics");

    public static Activity CreateConsumerActivity(ConsumeResult<string, string> consumeResult, string memberId)
    {
        var activity = source.CreateActivity($"{consumeResult.Topic} process", ActivityKind.Consumer);
        if (activity is null) return default;

        if (activity.IsAllDataRequested)
        {
            activity.SetTag(KafkaMessageTopic, consumeResult.Topic);
            activity.SetTag(KafkaMessagePartition, consumeResult.Partition.Value.ToString());
            activity.SetTag(KafkaMessageOffset, consumeResult.Offset.Value.ToString());
            activity.SetTag(KafkaMessageConsumerId, memberId);

            if (consumeResult.Message.Key is not null)
            {
                activity.SetTag(KafkaMessageKey, consumeResult.Message.Key);
            }
        }

        GetTraceHeaders(activity, consumeResult.Message);
        return activity.Start();
    }

    public static Activity CreateProducerActivity(string topic, Message<string, string> message)
    {
        var activity = source.StartActivity($"{topic} publish", ActivityKind.Producer);
        if (activity is null) return default;

        if (activity.IsAllDataRequested)
        {
            activity.SetTag(KafkaMessageTopic, topic);

            if (message.Key is not null)
            {
                activity.SetTag(KafkaMessageKey, message.Key);
            }
        }

        SetTraceHeaders(activity, message);
        return activity;
    }

    #region Non-Public Methods
    internal static void SetTraceHeaders(Activity activity, Message<string, string> message)
    {
        message.Headers ??= [];

        if (activity.Id is not null)
        {
            message.Headers.Add(TraceParentHeader, Encoding.UTF8.GetBytes(activity.Id));
        }

        var traceState = activity.Context.TraceState;
        if (traceState?.Length > 0)
        {
            message.Headers.Add(TraceStateHeader, Encoding.UTF8.GetBytes(activity.Context.TraceState));
        }
    }

    internal static void GetTraceHeaders(Activity activity, Message<string, string> message)
    {
        if (message.Headers is null) return;

        var traceParentHeader = message.Headers.FirstOrDefault(x => x.Key.Equals(TraceParentHeader));
        var traceStateHeader = message.Headers.FirstOrDefault(x => x.Key.Equals(TraceStateHeader));

        var traceParent = traceParentHeader is not null
            ? Encoding.UTF8.GetString(traceParentHeader.GetValueBytes())
            : null;

        var traceState = traceStateHeader is not null
            ? Encoding.UTF8.GetString(traceStateHeader.GetValueBytes())
            : null;

        if (ActivityContext.TryParse(traceParent, traceState, out var activityContext))
        {
            activity.SetParentId(activityContext.TraceId, activityContext.SpanId, activityContext.TraceFlags);
            activity.TraceStateString = activityContext.TraceState;
        }
    }
    #endregion
}
