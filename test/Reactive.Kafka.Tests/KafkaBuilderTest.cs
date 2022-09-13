using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using Reactive.Kafka.Configurations;

namespace Reactive.Kafka.Tests;

public class KafkaBuilderTest
{
    private readonly IServiceProvider provider;
    private readonly KafkaConfiguration configuration = new();

    public KafkaBuilderTest()
    {
        var serviceCollection = new ServiceCollection();

        serviceCollection.AddTransient(typeof(IConsumerWrapper<>), typeof(ConsumerWrapper<>));
        serviceCollection.AddTransient<IProducerWrapper, ProducerWrapper>();
        serviceCollection.AddSingleton<IList<IConsumerWrapper>, List<IConsumerWrapper>>();
        serviceCollection.AddSingleton<ILoggerFactory, LoggerFactory>();
        serviceCollection.AddSingleton<ILogger<ApplicationLifetime>, Logger<ApplicationLifetime>>();
        serviceCollection.AddSingleton<IHostApplicationLifetime, ApplicationLifetime>();

        Mock<IKafkaAdmin> kafkaAdmin;

        kafkaAdmin = new Mock<IKafkaAdmin>();
        kafkaAdmin
            .Setup(x => x.Partitions(It.IsAny<KafkaConfiguration>()))
            .Returns(1);

        serviceCollection.AddTransient(_ => kafkaAdmin.Object);

        provider = serviceCollection.BuildServiceProvider();
    }

    [Fact]
    public void ShouldCreateAppropriateConsumerPerPartition()
    {
        // Arrange
        configuration.Topic = "test-topic";
        configuration.ConsumerConfig.BootstrapServers = "localhost:9092";
        configuration.ConsumerConfig.GroupId = "test-group";

        // Act
        KafkaBuilder.BuildConsumerPerPartition<Consumer1, string>(provider, configuration);
        var sut = provider.GetRequiredService<IList<IConsumerWrapper>>();

        // Assert
        sut.Should().HaveCount(1);
    }

    [Fact]
    public void ShouldCreateAppropriateConsumerPerQuantity()
    {
        // Arrange
        configuration.Topic = "test-topic";
        configuration.ConsumerConfig.BootstrapServers = "localhost:9092";
        configuration.ConsumerConfig.GroupId = "test-group";

        // Act
        KafkaBuilder.BuildConsumerPerQuantity<Consumer1, string>(5, provider, configuration);
        var sut = provider.GetRequiredService<IList<IConsumerWrapper>>();

        // Assert
        sut.Should().HaveCount(5);
    }
}
