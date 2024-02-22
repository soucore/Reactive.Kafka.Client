using ConsumerWithTracing;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Reactive.Kafka.Extensions;
using Reactive.Kafka.Extensions.OpenTelemetry;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddOpenTelemetry()
            .WithTracing(tracerProviderBuilder => tracerProviderBuilder
                .ConfigureResource(r => r.AddService("consumer-with-tracing"))
                .AddJaegerExporter() // Jaeger to view traces
                .AddHttpClientInstrumentation()
                .AddReactiveKafkaInstrumentation());

        services.AddHttpClient("consumer-1", client =>
        {
            client.BaseAddress = new Uri("https://fakestoreapi.com");
        });

        services.AddReactiveKafka((provider, configurator) =>
        {
            configurator.AddConsumerPerPartition<Consumer1, string>("localhost:9092", "your-topic", "your-group");
        });
    })
    .Build();

await host.RunConsumersAsync();
await host.RunAsync();
