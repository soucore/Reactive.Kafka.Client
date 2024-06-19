using Reactive.Kafka.Enums;
using static Newtonsoft.Json.JsonConvert;
using static System.Text.Json.JsonSerializer;

namespace Reactive.Kafka.Helpers;

public static class Convert<T>
{
    private static readonly Type type = typeof(T);

    public static (bool Success, T Message, Exception ex) TryChangeType(string value, KafkaConfiguration configuration)
    {
        try
        {
            T output = (T)Convert.ChangeType(value, type);
            return (true, output, default);
        }
        catch (Exception ex)
        {
            return (false, default, ex);
        }
    }

    public static (bool Success, T Message, Exception ex) TrySerializeType(string value, KafkaConfiguration configuration)
    {
        try
        {
            T output;

            if (configuration.SerializerProvider == SerializerProvider.Newtonsoft)
                output = DeserializeObject<T>(value, configuration.JsonSerializerSettings);
            else
                output = Deserialize<T>(value, configuration.JsonSerializerOptions);

            return (true, output, default);
        }
        catch (Exception ex)
        {
            return (false, default, ex);
        }
    }

    public static (bool Success, string Message) TryStringType(string value, KafkaConfiguration configuration)
    {
        return (true, value);
    }
}
