using Reactive.Kafka.Enums;
using static Newtonsoft.Json.JsonConvert;
using static System.Text.Json.JsonSerializer;

namespace Reactive.Kafka.Helpers;

public static class Convert<T>
{
    public static (bool Success, T Message) TryChangeType(string value, KafkaConfiguration configuration)
    {
        try
        {
            T output = (T)Convert.ChangeType(value, typeof(T));
            return (true, output);
        }
        catch
        {
            return (false, default);
        }
    }

    public static (bool Success, T Message) TrySerializeType(string value, KafkaConfiguration configuration)
    {
        try
        {
            T output;

            if (configuration.SerializerProvider == SerializerProvider.Newtonsoft)
                output = DeserializeObject<T>(value, configuration.JsonSerializerSettings);
            else
                output = Deserialize<T>(value, configuration.JsonSerializerOptions);

            return (true, output);
        }
        catch
        {
            return (false, default);
        }
    }

    public static (bool Success, string Message) TryStringType(string value, KafkaConfiguration configuration)
    {
        return (true, value);
    }
}
