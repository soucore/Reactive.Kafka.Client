using System;

namespace Reactive.Kafka.Exceptions
{
    public class KafkaValidationException : Exception
    {
        public KafkaValidationException(string message) : base(message) { }
    }
}
