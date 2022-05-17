namespace Reactive.Kafka.Exceptions
{
    public class KafkaSerializationException : KafkaConsumerException
    {
        public KafkaSerializationException(string message, string messageKafka) 
            : base(message, messageKafka)
        {
        }
    }
}
