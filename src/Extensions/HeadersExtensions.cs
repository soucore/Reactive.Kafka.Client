namespace Reactive.Kafka.Extensions;

public static class HeadersExtensions
{
    public static string GetValue(this Headers headers, string key)
    {
        var header = headers
            .FirstOrDefault(x => x.Key.Equals(key));

        return header is null
            ? default
            : Encoding.UTF8.GetString(header.GetValueBytes());
    }

    public static void Add(this Headers headers, string key, string value)
    {
        headers.Add(key, Encoding.UTF8.GetBytes(value));
    }
}
