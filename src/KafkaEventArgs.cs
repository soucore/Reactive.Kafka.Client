namespace Reactive.Kafka
{
    public class KafkaEventArgs<T>
    {
        public KafkaEventArgs(string key, T message)
        {
            Key = key;
            Message = message;
        }
        public string Key { get; }
        public T Message { get; }
    }
}
