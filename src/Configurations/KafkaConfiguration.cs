namespace Reactive.Kafka.Configurations;

public class KafkaConfiguration
{
    public bool RespectObjectContract { get; set; } = false;
    public string Topic { get; set; }
    public ConsumerConfig ConsumerConfig { get; set; } = new();
}
