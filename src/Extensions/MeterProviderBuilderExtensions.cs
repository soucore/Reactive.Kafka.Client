namespace Reactive.Kafka.Extensions;

public static class MeterProviderBuilderExtensions
{
    public static MeterProviderBuilder AddReactiveKafkaInstrumentation(this MeterProviderBuilder builder)
    {
        return builder.AddMeter("Reactive.Kafka.Client");
    }
}
