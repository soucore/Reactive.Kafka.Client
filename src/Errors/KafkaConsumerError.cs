using System;

namespace Reactive.Kafka.Errors
{
    public sealed class KafkaConsumerError
    {
        public KafkaConsumerError(Exception ex)
        {
            Exception = ex;
            KafkaMessage = ex?.Message;
        }

        public Exception Exception { get; }
        public string KafkaMessage { get; }
    }
}
