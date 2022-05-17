namespace Reactive.Kafka.Exceptions
{
    public class KafkaConsumerException : Exception
    {
        public KafkaConsumerException(string message, string messageKafka) 
            : base(message) 
        {
            MessageKafka = messageKafka;
        }
        public string MessageKafka { get;}
    }
}
