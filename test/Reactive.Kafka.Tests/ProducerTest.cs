using System.Threading.Tasks;

namespace Reactive.Kafka.Tests;

public class ProducerTest
{
    private readonly IProducer<string, string> _producer
        = new ProducerBuilder<string, string>(new ProducerConfig
        {
            BootstrapServers = "localhost:9092"
        }).Build();

    private readonly ILoggerFactory _loggerFactory = new LoggerFactory();

    [Fact]
    public void InstantiateProducerWrapperCorrectly()
    {
        // Arrange
        var producerWrapper = new ProducerWrapper(_loggerFactory, _producer);

        // Assert
        Assert.NotNull(producerWrapper.Producer);
        Assert.IsAssignableFrom<IProducer<string, string>>(producerWrapper.Producer);
    }

    [Fact]
    public void EnsureOnProduceMethodCalledOnce()
    {
        // Arrange
        var mock = new Mock<IProducer<string, string>>();
        var message = new Message<string, string>() { Value = "test message." };

        mock.Setup(x => x.Produce("topic1", message, null));

        var producerWrapper = new ProducerWrapper(_loggerFactory, mock.Object);

        // Act
        producerWrapper.OnProduce("topic1", message);

        // Assert
        mock.Verify(x => x.Produce("topic1", message, null), Times.Once());
    }

    [Fact]
    public async Task EnsureOnProduceAsyncMethodCalledOnce()
    {
        // Arrange
        var mock = new Mock<IProducer<string, string>>();

        var message = new Message<string, string>() { Value = "test message." };
        var result = new DeliveryResult<string, string>();

        mock.Setup(x => x.ProduceAsync("topic1", message, default).Result)
            .Returns(result);

        var producerWrapper = new ProducerWrapper(_loggerFactory, mock.Object);

        // Act
        var dr = await producerWrapper.OnProduceAsync("topic1", message);

        // Assert
        Assert.Equal(result, dr);

        mock.Verify(x => x.ProduceAsync("topic1", message, default), Times.Once());
    }
}
