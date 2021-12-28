using Confluent.Kafka;

namespace Reactive.Kafka.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class KafkaConsumerConfigAttribute : Attribute
    {
        public KafkaConsumerConfigAttribute()
        {
        }

        public string GroupId { get; set; }

        public AutoOffsetReset AutoOffsetReset { get; set; }
    }
}
