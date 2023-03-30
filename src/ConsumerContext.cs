namespace Reactive.Kafka;

public sealed class ConsumerContext
{
    public IConsumer<string, string> Consumer { get; internal set; }
    public ConsumeResult<string, string> ConsumeResult { get; internal set; }
    public Exception Exception { get; internal set; }

    public List<TopicPartitionOffset> Commit()
        => Consumer?.Commit();

    public void Commit(IEnumerable<TopicPartitionOffset> offsets)
        => Consumer?.Commit(offsets);

    public void Commit(ConsumeResult<string, string> result)
        => Consumer?.Commit(result);

    internal void SetResult(ConsumeResult<string, string> consumerResult)
    {
        ConsumeResult = consumerResult;
        Exception = null;
    }
}
