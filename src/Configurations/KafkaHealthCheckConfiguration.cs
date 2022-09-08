namespace Reactive.Kafka.Configurations;

public class KafkaHealthCheckConfiguration
{
    public KafkaHealthCheckConfiguration(IConfiguration config)
    {
        ReferenceTimeMinutes = config
            .GetValue("KafkaHealthCheck:ReferenceTimeMinutes", 10);

        IntervalSeconds = config
            .GetValue("KafkaHealthCheck:IntervalSeconds", 10);

        NumberOfObservedConsumers = config
            .GetValue("KafkaHealthCheck:NumberOfObservedConsumers", 1);
    }

    public int ReferenceTimeMinutes { get; set; }
    public int IntervalSeconds { get; set; }
    public int NumberOfObservedConsumers { get; set; }
}
