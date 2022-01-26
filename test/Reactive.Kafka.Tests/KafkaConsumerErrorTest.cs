using System;
using Reactive.Kafka.Errors;
using Reactive.Kafka.Exceptions;
using Xunit;

namespace Reactive.Kafka.Tests;

public class KafkaConsumerErrorTest
{
    [Fact]
    public void PlacesExceptionAndChecksPropertyValueValid()
    {
        // Arrange
        var value = new Exception("Exception generated");
        const string expected = "Exception generated";

        // Act
        var consumerError = new KafkaConsumerError(value);

        // Assert
        Assert.NotNull(consumerError.Exception);
        Assert.True(consumerError.KafkaMessage == expected);
    }
    
    [Fact]
    public void PlacesExceptionAndChecksPropertyValueInvalid()
    {
        // Arrange
        var value = new Exception("Exception generated Error");
        const string expected = "Exception generated";

        // Act
        var consumerError = new KafkaConsumerError(value);

        // Assert
        Assert.NotNull(consumerError.Exception);
        Assert.False(consumerError.KafkaMessage == expected);
    }
    
    [Fact]
    public void PlacesCustomExceptionAndChecksPropertyValueValid()
    {
        // Arrange
        var value = new KafkaConsumerException("Exception generated");
        const string expected = "Exception generated";

        // Act
        var consumerError = new KafkaConsumerError(value);

        // Assert
        Assert.NotNull(consumerError.Exception);
        Assert.True(consumerError.KafkaMessage == expected);
    }
    
    [Fact]
    public void PlacesCustomExceptionAndChecksPropertyValueInValid()
    {
        // Arrange
        var value = new KafkaConsumerException("Exception generated Error");
        const string expected = "Exception generated";

        // Act
        var consumerError = new KafkaConsumerError(value);

        // Assert
        Assert.NotNull(consumerError.Exception);
        Assert.False(consumerError.KafkaMessage == expected);
    }
    
    [Fact]
    public void ParamExceptionIsNull()
    {
        // Arrange
        Exception value = null;
        string expected = null;
        
        // Act
        var consumerError = new KafkaConsumerError(value);

        // Assert
        Assert.Null(consumerError.Exception);
        Assert.True(consumerError.KafkaMessage == expected);
    }
}