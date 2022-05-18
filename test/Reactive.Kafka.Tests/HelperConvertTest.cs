using static Reactive.Kafka.Helpers.Convert;

namespace Reactive.Kafka.Tests;

public class HelperConvertTest
{
    [Fact]
    public void TryChangeTypeIsTrue()
    {
        // Arrange
        const int value = 2;
        const string expected = "2";

        // Act
        var isChange = TryChangeType(value, out string output);

        // Assert
        Assert.True(isChange);
        Assert.True(output == expected);
    }

    [Fact]
    public void TryChangeTypeIsFalse()
    {
        // Arrange
        List<int> value = new();
        const int expected = 0;

        // Act
        var isChange = TryChangeType(value, out int output);

        // Assert
        Assert.False(isChange);
        Assert.True(output == expected);
    }

    [Fact]
    public void TrySerializeTypeIsTrue()
    {
        // Arrange 
        const string str = "{\"id\":0, \"name\": \"Kafka\"}";
        const string expected = "MessageTest { Id = 0, Name = Kafka }";

        // Act
        var result = TrySerializeType(str, false, out MessageTest output);

        // Assert
        Assert.True(result);
        Assert.True(output.ToString() == expected);

    }

    [Fact]
    public void TrySerializeTypeIsFalse()
    {
        // Arrange 
        const string str = "valor";

        // Act
        var result = TrySerializeType(str, false, out MessageTest output);

        // Assert
        Assert.False(result);
        Assert.Null(output);
    }
}