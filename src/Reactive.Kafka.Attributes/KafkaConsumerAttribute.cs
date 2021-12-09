using System;

namespace Reactive.Kafka.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ReactiveKafkaConsumerAttribute : Attribute
    {
        public bool Enabled = true;
    }
}