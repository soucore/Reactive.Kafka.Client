using System.Collections.Generic;
using Reactive.Kafka.Helpers;
using Reactive.Kafka.Tests.Types;
using Xunit;

namespace Reactive.Kafka.Tests;

public class HelperConvertTest
{
    [Fact]
    public void  TryChangeTypeIsTrue()
    {
        // Arrange
        const int value = 2;
        const string expected = "2";
        
        // Act
        var isChange = Convert.TryChangeType(value, out string output);
        
        // Assert
        Assert.True(isChange);
        Assert.True(output == expected);
    }
    
    [Fact]
    public void  TryChangeTypeIsFalse()
    {
        // Arrange
        List<int> value = new();
        
        // Act
        var isChange = Convert.TryChangeType(value, out int _);
        
        // Assert
        Assert.False(isChange);
    }
    
    [Fact]
    public void  TrySerializeTypeIsTrue()
    {
        // Arrange 
        const string str = "{\"id\":0, \"name\": \"Kafka\"}";
        const string expected = "MessageTest { Id = 0, Name = Kafka }";
        
        // Act
        var result = Convert.TrySerializeType(str, out MessageTest output);
        
        // Assert
        Assert.True(result);
        Assert.True(output.ToString() == expected);

    }
    
    [Fact]
    public void  TrySerializeTypeIsFalse()
    {
        // Arrange 
        const string str = "valor";

        // Act
        var result = Convert.TrySerializeType(str, out MessageTest output);
        
        // Assert
        Assert.False(result);
    }
}