using Reactive.Kafka.Tests.Asserts;

namespace Reactive.Kafka.Tests.Extensions;

public static class TestTools
{
    public static void WithMessage<T>(this T exception, string message) where T : Exception
    {
        if (exception.Message.Equals(message))
        {
            Assert.True(true, $"Actual message matches the expected one: {message}");
        }
        else
        {
            AssertCustom.Fail(message);
        }
    }
}