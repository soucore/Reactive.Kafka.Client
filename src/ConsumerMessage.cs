namespace Reactive.Kafka;

public sealed class ConsumerMessage<T>(string key, T message)
{
    public string Key { get; } = key;
    public T Value { get; } = message;
}
