namespace Reactive.Kafka.Exceptions
{
    public class KafkaConsumerException : Exception
    {
        public KafkaConsumerException(string kafkaMessage)
            => KafkaMessage = kafkaMessage;

        public string KafkaMessage { get; set; }
    }
}
