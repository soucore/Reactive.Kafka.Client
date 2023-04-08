using Newtonsoft.Json;
using Reactive.Kafka.Enums;
using System.Text.Json;

namespace Reactive.Kafka.Configurations;

public class KafkaConfiguration
{
    public string Topic { get; set; }
    public ConsumerConfig ConsumerConfig { get; set; } = new();

    internal JsonSerializerSettings JsonSerializerSettings { get; set; }
    internal JsonSerializerOptions JsonSerializerOptions { get; set; }
    internal SerializerProvider SerializerProvider { get; set; } = SerializerProvider.Newtonsoft;

    public void UseNewtonsoft(Action<JsonSerializerSettings> action = null)
    {
        SerializerProvider = SerializerProvider.Newtonsoft;
        action?.Invoke(JsonSerializerSettings ??= new());
    }

    public void UseSystemTextJson(Action<JsonSerializerOptions> action = null)
    {
        SerializerProvider = SerializerProvider.SystemTextJson;
        action?.Invoke(JsonSerializerOptions ??= new());
    }
}
