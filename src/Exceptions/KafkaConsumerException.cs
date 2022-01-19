using System;

namespace Reactive.Kafka.Exceptions
{
    public class KafkaConsumerException : Exception
    {
        public KafkaConsumerException(string message) : base(message) { }
    }
}
