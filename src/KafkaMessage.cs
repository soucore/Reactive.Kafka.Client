namespace Reactive.Kafka
{
    public sealed class KafkaMessage<T>
    {
        public KafkaMessage(string key, T message)
        {
            Key = key;
            Message = message;
        }

        public string Key { get; }
        public T Message { get; }
    }
}
