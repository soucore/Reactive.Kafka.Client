namespace Reactive.Kafka.Helpers;

internal static class ActivityHelper
{
    private static readonly CompositeTextMapPropagator propagator = new([
        new TraceContextPropagator(),
        new BaggagePropagator()
    ]);

    private static readonly ActivitySource _source = new("Reactive.Kafka.Client");

    public static Activity CreateConsumerActivity(ConsumeResult<string, string> consumeResult, string memberId, string groupId)
    {
        if (!_source.HasListeners())
        {
            return default;
        }

        var activityName = $"{consumeResult.Topic} process";
        var activityKind = ActivityKind.Consumer;

        var activity = _source.CreateActivity(activityName, activityKind);
        if (activity is null)
        {
            return default;
        }

        if (activity.IsAllDataRequested)
        {
            activity.SetTag(MessagingSemanticConventions.MessagingSystem, "kafka");
            activity.SetTag(MessagingSemanticConventions.MessagingDestName, consumeResult.Topic);
            activity.SetTag(MessagingSemanticConventions.MessagingKafkaConsumerGroup, groupId);
            activity.SetTag(MessagingSemanticConventions.MessagingClientId, memberId);

            SetPartitionOffsetTags(activity, consumeResult.TopicPartitionOffset);

            if (consumeResult.Message.Key is not null)
            {
                activity.SetTag(MessagingSemanticConventions.MessagingKafkaMessageKey, consumeResult.Message.Key);
            }
        }

        ExtractTraceContext(activity, consumeResult.Message);
        return activity.Start();
    }

    public static Activity CreateProducerActivity(string topic, Message<string, string> message, string producerName)
    {
        if (!_source.HasListeners())
        {
            return default;
        }

        var activityName = $"{topic} publish";
        var activityKind = ActivityKind.Producer;

        var activity = _source.StartActivity(activityName, activityKind);
        if (activity is null)
        {
            return default;
        }

        if (activity.IsAllDataRequested)
        {
            activity.SetTag(MessagingSemanticConventions.MessagingSystem, "kafka");
            activity.SetTag(MessagingSemanticConventions.MessagingDestName, topic);
            activity.SetTag(MessagingSemanticConventions.MessagingClientId, producerName);

            if (message.Key is not null)
            {
                activity.SetTag(MessagingSemanticConventions.MessagingKafkaMessageKey, message.Key);
            }
        }

        InjectTraceContext(activity, message);
        return activity;
    }

    #region Non-Public Methods
    internal static void SetPartitionOffsetTags(Activity activity, TopicPartitionOffset topicPartitionOffset)
    {
        if (activity is not null and { IsAllDataRequested: true })
        {
            activity.SetTag(MessagingSemanticConventions.MessagingKafkaDestPartition, topicPartitionOffset.Partition.Value.ToString());
            activity.SetTag(MessagingSemanticConventions.MessagingKafkaMessageOffset, topicPartitionOffset.Offset.Value.ToString());
        }
    }

    internal static void InjectTraceContext(Activity activity, Message<string, string> message)
    {
        message.Headers ??= [];

        var propagationContext = new PropagationContext(activity.Context, Baggage.Current);
        var carrier = message.Headers;

        propagator.Inject(propagationContext, carrier, (headers, k, v) => headers.Add(k, v));
    }

    internal static void ExtractTraceContext(Activity activity, Message<string, string> message)
    {
        if (message.Headers is null or { Count: 0 })
        {
            return;
        }

        var propagationContext = propagator
            .Extract(default, message.Headers, (headers, k) => [headers.GetValue(k)]);

        if (propagationContext.ActivityContext.IsValid())
        {
            var ctx = propagationContext.ActivityContext;

            activity.SetParentId(ctx.TraceId, ctx.SpanId, ctx.TraceFlags);
            activity.TraceStateString = ctx.TraceState;
        }

        if (propagationContext.Baggage != default)
        {
            Baggage.Current = propagationContext.Baggage;
        }
    }
    #endregion

    // Semantic Conventions for Kafka
    // https://opentelemetry.io/docs/specs/semconv/messaging/kafka/
    private static class MessagingSemanticConventions
    {
        public const string MessagingDestName = "messaging.destination.name";
        public const string MessagingKafkaConsumerGroup = "messaging.kafka.consumer.group";
        public const string MessagingKafkaDestPartition = "messaging.kafka.destination.partition";
        public const string MessagingKafkaMessageOffset = "messaging.kafka.message.offset";
        public const string MessagingClientId = "messaging.client_id";
        public const string MessagingKafkaMessageKey = "messaging.kafka.message.key";
        public const string MessagingSystem = "messaging.system";
    }
}
