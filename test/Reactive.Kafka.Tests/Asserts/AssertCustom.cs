using Xunit;
using Xunit.Sdk;

namespace Reactive.Kafka.Tests.Asserts;

public static class AssertCustom
{
    public static void Fail(string message)
        => throw new XunitException(message);
}