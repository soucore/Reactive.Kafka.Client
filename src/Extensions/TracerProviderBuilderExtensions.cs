namespace Reactive.Kafka.Extensions.OpenTelemetry;

public static class TracerProviderBuilderExtensions
{
    public static TracerProviderBuilder AddReactiveKafkaInstrumentation(this TracerProviderBuilder builder)
    {
        return builder.AddSource("Reactive.Kafka.Client");
    }
}
