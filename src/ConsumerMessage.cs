namespace Reactive.Kafka;

public sealed class ConsumerMessage<T>
{
    public ConsumerMessage(string key, T message)
    {
        Key = key;
        Message = message;
    }

    public string Key { get; }
    public T Message { get; }
}
