using Reactive.Kafka.Enums;

namespace Reactive.Kafka.Configurations;

public class KafkaConfiguration
{
    public bool RespectObjectContract { get; set; } = false;
    public string Topic { get; set; }
    public SerializerProvider SerializerProvider { get; set; } = SerializerProvider.Newtonsoft;
    public ConsumerConfig ConsumerConfig { get; set; } = new();
}
