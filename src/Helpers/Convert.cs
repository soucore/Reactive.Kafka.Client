using ServiceStack.Text;

namespace Reactive.Kafka.Helpers
{
    public static class Convert
    {
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

        public static bool TrySerializeType<T>(string value, out T output)
        {
            output = default;

            try
            {
                output = JsonSerializer.DeserializeFromString<T>(value);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
