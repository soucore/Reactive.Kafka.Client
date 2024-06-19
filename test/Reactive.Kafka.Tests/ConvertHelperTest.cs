using Reactive.Kafka.Helpers;

namespace Reactive.Kafka.Tests;

public class ConvertHelperTest
{
    [Fact]
    public void TryChangeTypeIsTrue()
    {
        // Arrange
        const string value = "2";
        const int expected = 2;

        // Act
        (bool success, int result, _) = Convert<int>.TryChangeType(value, new());

        // Assert
        Assert.True(success);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void TryChangeTypeIsFalse()
    {
        // Arrange
        const string value = "{}";
        const int expected = 0;

        // Act
        (bool success, int result, _) = Convert<int>.TryChangeType(value, new());

        // Assert
        Assert.False(success);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void TrySerializeTypeIsTrue()
    {
        // Arrange 
        const string str = "{\"id\":0, \"name\": \"Kafka\"}";
        const string expected = "MessageTest { Id = 0, Name = Kafka }";

        // Act
        (bool success, MessageTest result, _) = Convert<MessageTest>.TrySerializeType(str, new());

        // Assert
        Assert.True(success);
        Assert.Equal(expected, result.ToString());
    }

    [Fact]
    public void TrySerializeTypeIsFalse()
    {
        // Arrange 
        const string str = "valor";

        // Act
        (bool success, MessageTest result, _) = Convert<MessageTest>.TrySerializeType(str, new());

        // Assert
        Assert.False(success);
        Assert.Null(result);
    }
}
