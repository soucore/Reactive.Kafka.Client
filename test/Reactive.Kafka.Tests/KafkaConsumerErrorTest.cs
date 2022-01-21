using System;
using Reactive.Kafka.Errors;
using Reactive.Kafka.Exceptions;
using Xunit;

namespace Reactive.Kafka.Tests;

public class KafkaConsumerErrorTest
{
    [Fact]
    public void PlacesExceptionAndChecksPropertyValue()
    {
        // Arrange
        const string expected = "Exception generated";
        var value = new Exception("Exception generated");
        var value2 = new KafkaConsumerException("Exception generated");
        
        // Act
        var consumerError = new KafkaConsumerError(value);
        var consumerError2 = new KafkaConsumerError(value2);

        // Assert
        Assert.NotNull(consumerError.Exception);
        Assert.True(consumerError.KafkaMessage == expected);
        Assert.NotNull(consumerError2.Exception);
        Assert.True(consumerError2.KafkaMessage == expected);
    }
}