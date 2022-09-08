﻿using Reactive.Kafka.Configurations;
using Reactive.Kafka.Extensions;

namespace Reactive.Kafka.Tests;

public class ConsumerConfiguratorTest
{
    private readonly IServiceProvider provider;
    private readonly ConsumerConfigurator consumerConfigurator;

    public ConsumerConfiguratorTest()
    {
        var serviceCollection = new ServiceCollection();

        serviceCollection.AddTransient(typeof(IConsumerWrapper<>), typeof(ConsumerWrapper<>));
        serviceCollection.AddTransient<IProducerWrapper, ProducerWrapper>();
        serviceCollection.AddSingleton<IList<IConsumerWrapper>, List<IConsumerWrapper>>();
        serviceCollection.AddSingleton<ILoggerFactory, LoggerFactory>();

        Mock<IKafkaAdmin> kafkaAdmin;

        kafkaAdmin = new Mock<IKafkaAdmin>();
        kafkaAdmin
            .Setup(x => x.Partitions(It.IsAny<KafkaConfiguration>()))
            .Returns(1);

        serviceCollection.AddTransient(_ => kafkaAdmin.Object);

        provider = serviceCollection.BuildServiceProvider();
        consumerConfigurator = provider.CreateInstance<ConsumerConfigurator>(provider);
    }

    [Fact]
    public void ShouldCreateAppropriateConsumerPerQuantity()
    {
        // Arrange & Act
        consumerConfigurator.AddConsumerPerQuantity<Consumer1, string>("localhost", 1, "test-topic", "test-group");
        var sut = provider.GetRequiredService<IList<IConsumerWrapper>>();

        // Assert
        sut.Should().HaveCount(1);
        sut.Should().ContainItemsAssignableTo<IConsumerWrapper<string>>();
    }

    [Fact]
    public void ShouldCreateAppropriateConfigurableConsumerPerQuantity()
    {
        // Arrange & Act
        consumerConfigurator.AddConsumerPerQuantity<Consumer1, string>("localhost", 2, (provider, config) =>
        {
            config.Topic = "test-topic";
            config.ConsumerConfig.GroupId = "test-group";
            config.ConsumerConfig.BootstrapServers = "localhost:9092";
        });

        var sut = provider.GetRequiredService<IList<IConsumerWrapper>>();

        // Assert
        sut.Should().HaveCount(2);
        sut.Should().ContainItemsAssignableTo<IConsumerWrapper<string>>();
    }

    [Fact]
    public void ShouldCreateAppropriateConsumerPerPartition()
    {
        // Arrange & Act
        consumerConfigurator.AddConsumerPerPartition<Consumer1, string>("localhost", "test-topic", "test-group");
        var sut = provider.GetRequiredService<IList<IConsumerWrapper>>();

        // Assert
        sut.Should().HaveCount(1);
        sut.Should().ContainItemsAssignableTo<IConsumerWrapper<string>>();
    }

    [Fact]
    public void ShouldCreateAppropriateConfigurableConsumerPerPartition()
    {
        // Arrange & Act
        consumerConfigurator.AddConsumerPerPartition<Consumer1, string>("localhost", (provider, config) =>
        {
            config.Topic = "test-topic";
            config.ConsumerConfig.GroupId = "test-group";
            config.ConsumerConfig.BootstrapServers = "localhost:9092";
        });

        var sut = provider.GetRequiredService<IList<IConsumerWrapper>>();

        // Assert
        sut.Should().HaveCount(1);
        sut.Should().ContainItemsAssignableTo<IConsumerWrapper<string>>();
    }

    private static PartitionMetadata GetPartitionMetadata(int partitionId)
    {
        return new(1, 1, Array.Empty<int>(), Array.Empty<int>(), null);
    }

    private static TopicMetadata GetTopicMetadata(string topic)
    {
        List<PartitionMetadata> partitions = new() { GetPartitionMetadata(1) };
        return new(topic, partitions, null);
    }

    private static Metadata GetMetadata(string topic)
    {
        List<TopicMetadata> topicMetadata = new() { GetTopicMetadata("test-topic") };
        return new(null, topicMetadata, 0, null);
    }
}
