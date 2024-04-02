namespace Reactive.Kafka.Helpers;

internal static class MeterHelper
{
    private static readonly Meter _meter = new("Reactive.Kafka.Client");

    #region Metrics
    private static readonly Histogram<long> _consumerLag = _meter.CreateHistogram<long>("messaging.kafka.consumer.lag", "ms", "");
    private static readonly Histogram<long> _processDuration = _meter.CreateHistogram<long>("messaging.kafka.consumer.process.duration", "ms", "");
    private static readonly Histogram<long> _publishDuration = _meter.CreateHistogram<long>("messaging.kafka.producer.publish.duration", "ms", "");
    #endregion

    internal static void RecordConsumerLag(ConsumeResult<string, string> consumeResult)
    {
        if (!_consumerLag.Enabled) return;

        long insertedOn = consumeResult.Message.Timestamp.UnixTimestampMs;
        long receivedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        TagList tags = new()
        {
            { "messaging.source.name", consumeResult.Topic },
            { "messaging.kafka.destination.partition", consumeResult.Partition.Value.ToString() }
        };

        _consumerLag.Record(receivedAt - insertedOn, tags);
    }

    internal static MeterDuration RecordConsumerProcessDuration(ConsumeResult<string, string> consumeResult)
    {
        if (!_processDuration.Enabled)
        {
            return default;
        }

        return new MeterDuration(_processDuration, [
            new("messaging.source.name", consumeResult.Topic),
        ]);
    }

    internal static MeterDuration RecordProducerPublishDuration(string topic)
    {
        if (!_publishDuration.Enabled)
        {
            return default;
        }

        return new MeterDuration(_publishDuration, [
            new("messaging.destination.name", topic),
        ]);
    }

    internal class MeterDuration(Histogram<long> histogram, KeyValuePair<string, object>[] tags) : IDisposable
    {
        private readonly Stopwatch stopwatch = Stopwatch.StartNew();

        public void Dispose()
        {
            stopwatch.Stop();
            histogram.Record(stopwatch.ElapsedMilliseconds, tags);
        }
    }
}
