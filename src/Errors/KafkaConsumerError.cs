namespace Reactive.Kafka.Errors
{
    public class KafkaConsumerError
    {
        public KafkaConsumerError(KafkaConsumerException ex)
        {
            Exception = ex;
            KafkaMessage = ex?.MessageKafka;
        }

        public KafkaConsumerError(Exception ex)
        {
            Exception = ex;
            KafkaMessage = ex?.Message;
        }

        public Exception Exception { get; }
        public string KafkaMessage { get; }
    }
}
