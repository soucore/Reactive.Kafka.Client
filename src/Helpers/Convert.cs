using Newtonsoft.Json;

namespace Reactive.Kafka.Helpers;

public static class Convert
{
    private readonly static JsonSerializerSettings settings = new()
    {
        MissingMemberHandling = MissingMemberHandling.Error
    };

    public static bool TryChangeType<T>(object value, out T output)
    {
        output = default;

        try
        {
            output = (T)System.Convert.ChangeType(value, typeof(T));
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static bool TrySerializeType<T>(string value, bool respectObjectContract, out T output)
    {
        output = default;

        try
        {
            output = JsonConvert.DeserializeObject<T>(value, respectObjectContract ? settings : null);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
