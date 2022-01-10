using System;

namespace Reactive.Kafka.Errors
{
    public class KafkaConsumerError
    {
        public Exception Exception { get; set; }
        public string KafkaMessage { get; set; }
    }
}
