using System.Threading.Tasks;

namespace Reactive.Kafka.Tests;

public class ConsumerBaseTest
{
    [Fact]
    public void SyncShortcutProduceEvent()
    {
        // Arrange
        ConsumerBase<string> consumerBase = new Consumer2();

        string expectedTopic = "";
        string expectedMessage = "";

        consumerBase.OnProduce += (topic, message) =>
        {
            expectedTopic = topic;
            expectedMessage = message.Value;
        };

        // Act
        consumerBase.Produce("topic1", "test message.");

        // Assert
        Assert.NotEmpty(expectedTopic);
        Assert.NotEmpty(expectedMessage);
        Assert.Equal("topic1", expectedTopic);
        Assert.Equal("test message.", expectedMessage);
    }

    [Fact]
    public async Task AsyncShortcutProduceEvent()
    {
        // Arrange
        ConsumerBase<string> consumerBase = new Consumer2();

        string expectedTopic = "";
        string expectedMessage = "";

        consumerBase.OnProduceAsync += (topic, message) =>
        {
            expectedTopic = topic;
            expectedMessage = message.Value;

            return Task.FromResult(new DeliveryResult<string, string>());
        };

        // Act
        await consumerBase.ProduceAsync("topic1", "test message.");

        // Assert
        Assert.NotEmpty(expectedTopic);
        Assert.NotEmpty(expectedMessage);
        Assert.Equal("topic1", expectedTopic);
        Assert.Equal("test message.", expectedMessage);
    }

    [Fact]
    public void SyncProduceEvent()
    {
        // Arrange
        ConsumerBase<string> consumerBase = new Consumer2();

        string expectedTopic = "";
        string expectedMessage = "";

        consumerBase.OnProduce += (topic, message) =>
        {
            expectedTopic = topic;
            expectedMessage = message.Value;
        };

        // Act
        consumerBase.Produce("topic1", new Message<string, string>
        {
            Value = "test message."
        });

        // Assert
        Assert.NotEmpty(expectedTopic);
        Assert.NotEmpty(expectedMessage);
        Assert.Equal("topic1", expectedTopic);
        Assert.Equal("test message.", expectedMessage);
    }

    [Fact]
    public async Task AsyncProduceEvent()
    {
        // Arrange
        ConsumerBase<string> consumerBase = new Consumer2();

        string expectedTopic = "";
        string expectedMessage = "";

        consumerBase.OnProduceAsync += (topic, message) =>
        {
            expectedTopic = topic;
            expectedMessage = message.Value;

            return Task.FromResult(new DeliveryResult<string, string>());
        };

        // Act
        await consumerBase.ProduceAsync("topic1", new Message<string, string>
        {
            Value = "test message."
        });

        // Assert
        Assert.NotEmpty(expectedTopic);
        Assert.NotEmpty(expectedMessage);
        Assert.Equal("topic1", expectedTopic);
        Assert.Equal("test message.", expectedMessage);
    }

    [Fact]
    public void OnBeforeSerializationMethod()
    {
        // Arrange
        ConsumerBase<string> consumerBase = new Consumer2();

        string expectedMessage = "raw message.";

        // Act
        string returnMessage = consumerBase
            .OnBeforeSerialization("raw message.");

        // Assert
        Assert.Equal(expectedMessage, returnMessage);
    }

    [Fact]
    public void OnAfterSerializationMethod()
    {
        // Arrange
        ConsumerBase<string> consumerBase = new Consumer2();

        string expectedMessage = "message.";

        // Act
        string returnMessage = consumerBase
            .OnAfterSerialization("message.");

        // Assert
        Assert.Equal(expectedMessage, returnMessage);
    }

    [Fact]
    public void OnConsumerBuilderMethod()
    {
        // Arrange
        ConsumerBase<string> consumerBase = new Consumer3();
        ConsumerConfig config = new();

        string expectedBootstrapServer = "localhost:9092";
        string expectedGroup = "Group";

        // Act
        consumerBase.OnConsumerConfiguration(config);

        // Assert
        Assert.Equal(expectedBootstrapServer, config.BootstrapServers);
        Assert.Equal(expectedGroup, config.GroupId);
    }

    [Fact]
    public void OnProducerBuilderMethod()
    {
        // Arrange
        ConsumerBase<string> consumerBase = new Consumer3();
        ProducerConfig config = new();

        string expectedBootstrapServer = "localhost:9092";
        Acks expectedAckMode = Acks.None;

        // Act
        consumerBase.OnProducerConfiguration(config);

        // Assert
        Assert.Equal(expectedBootstrapServer, config.BootstrapServers);
        Assert.Equal(expectedAckMode, config.Acks);
    }
}
