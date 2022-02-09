using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Reactive.Kafka.Extensions;
using Reactive.Kafka.Tests.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace Reactive.Kafka.Tests
{
    public class KafkaReflectionTest
    {
        private IServiceProvider provider;

        public KafkaReflectionTest()
        {
            // Setup
            var services = new ServiceCollection();

            services.AddSingleton<ILoggerFactory, LoggerFactory>();
            services.AddTransient(provider =>
            {
                return new ConsumerConfig()
                {
                    BootstrapServers = "localhost:9092",
                    GroupId = "Group"
                };
            });

            provider = services.BuildServiceProvider();
        }

        [Fact]
        public void ConsumerWithoutTopicSubscription()
        {
            // Act
            IConsumer<string, string> consumer = ServiceCollectionExtensions
                .ApplyReflection(provider, typeof(Consumer1), test: true);

            // Assert
            Assert.Null(consumer);
        }

        [Fact]
        public void ConsumerWithTopicSubscription()
        {
            // Act
            IConsumer<string, string> consumer = ServiceCollectionExtensions
                .ApplyReflection(provider, typeof(Consumer2), test: true);

            // Assert
            Assert.NotEmpty(consumer.Subscription);
            Assert.Single(consumer.Subscription);
            Assert.NotNull(consumer.Subscription.FirstOrDefault(topic => topic == "test-topic"));
        }

        [Fact]
        public void EnsuresConsumerWrapperHasBeenCreated()
        {
            ServiceCollectionExtensions.listConsumerWrapper.Clear();
            ServiceCollectionExtensions.partitionsDict.Clear();

            // Act
            IConsumer<string, string> consumer = ServiceCollectionExtensions
                .ApplyReflection(provider, typeof(Consumer2), test: true);

            // Assert
            Assert.NotEmpty(ServiceCollectionExtensions.listConsumerWrapper);
            Assert.Single(ServiceCollectionExtensions.listConsumerWrapper);
        }

        [Fact]
        public async Task TryConnectWrongKafkaServer()
        {
            // Arrange
            Func<Task> function = () =>
            {
                ServiceCollectionExtensions.PartitionsDiscovery("wrongserver:9092");
                return Task.CompletedTask;
            };

            // Assert
            await Assert.ThrowsAsync<KafkaException>(function);
        }

        [Fact]
        public void PartitionsDiscoveryTest()
        {
            ServiceCollectionExtensions.listConsumerWrapper.Clear();
            ServiceCollectionExtensions.partitionsDict.Clear();

            // Arrange
            List<PartitionMetadata> partitions1 = new()
            {
                new PartitionMetadata(1, 1, Array.Empty<int>(), Array.Empty<int>(), null),
                new PartitionMetadata(2, 1, Array.Empty<int>(), Array.Empty<int>(), null),
                new PartitionMetadata(3, 1, Array.Empty<int>(), Array.Empty<int>(), null)
            };

            List<PartitionMetadata> partitions2 = new()
            {
                new PartitionMetadata(4, 1, Array.Empty<int>(), Array.Empty<int>(), null),
                new PartitionMetadata(5, 1, Array.Empty<int>(), Array.Empty<int>(), null),
            };

            List<TopicMetadata> topics = new()
            {
                new TopicMetadata("topic1", partitions1, null),
                new TopicMetadata("topic2", partitions2, null)
            };

            Metadata meta = new(new(), topics, 0, null);

            int expectedTopic1 = 3;
            int expectedTopic2 = 2;

            // Act
            ServiceCollectionExtensions.FillPartitionsDict(meta);

            // Assert
            Assert.True(ServiceCollectionExtensions.partitionsDict.ContainsKey("topic1"));
            Assert.True(ServiceCollectionExtensions.partitionsDict.ContainsKey("topic2"));
            Assert.NotEmpty(ServiceCollectionExtensions.partitionsDict);
            Assert.Equal(expectedTopic1, ServiceCollectionExtensions.partitionsDict["topic1"]);
            Assert.Equal(expectedTopic2, ServiceCollectionExtensions.partitionsDict["topic2"]);
        }

        [Fact]
        public void EnsureOneConsumerPerPartition()
        {
            ServiceCollectionExtensions.listConsumerWrapper.Clear();
            ServiceCollectionExtensions.partitionsDict.Clear();

            // Arrange
            List<PartitionMetadata> partitions = new()
            {
                new PartitionMetadata(1, 1, Array.Empty<int>(), Array.Empty<int>(), null),
                new PartitionMetadata(2, 1, Array.Empty<int>(), Array.Empty<int>(), null),
                new PartitionMetadata(3, 1, Array.Empty<int>(), Array.Empty<int>(), null)
            };

            List<TopicMetadata> topics = new()
            {
                new TopicMetadata("test-topic", partitions, null),
            };

            Metadata meta = new(new(), topics, 0, null);

            int consumersExpected = partitions.Count; // 3

            // Act
            ServiceCollectionExtensions.FillPartitionsDict(meta);
            ServiceCollectionExtensions.ApplyConsumerPerPartition(provider, typeof(Consumer2), test: true);

            // Assert
            Assert.NotEmpty(ServiceCollectionExtensions.listConsumerWrapper);
            Assert.Equal(consumersExpected, ServiceCollectionExtensions.listConsumerWrapper.Count);
        }

        [Fact]
        public void EnsureConsumersPerQuantity()
        {
            ServiceCollectionExtensions.listConsumerWrapper.Clear();
            ServiceCollectionExtensions.partitionsDict.Clear();

            // Arrange
            int consumersExpected = 2;

            // Act
            ServiceCollectionExtensions.ApplyConsumerPerQuantity(provider, typeof(Consumer2), quantity: 2, test: true);

            // Assert
            Assert.NotEmpty(ServiceCollectionExtensions.listConsumerWrapper);
            Assert.Equal(consumersExpected, ServiceCollectionExtensions.listConsumerWrapper.Count);
        }

        [Fact]
        public void EnsureConsumersAssembly()
        {
            ServiceCollectionExtensions.listConsumerWrapper.Clear();
            ServiceCollectionExtensions.partitionsDict.Clear();

            // Arrange
            int consumersExpected = 2; 
            int consumersNotExpected = 3; // There are 3 consumers but one isn't assigned 

            // Act
            ServiceCollectionExtensions.ApplyConsumersFromAssembly(provider, typeof(KafkaReflectionTest).Assembly, test: true);

            // Assert
            Assert.NotEmpty(ServiceCollectionExtensions.listConsumerWrapper);
            Assert.NotEqual(consumersNotExpected, ServiceCollectionExtensions.listConsumerWrapper.Count);
            Assert.Equal(consumersExpected, ServiceCollectionExtensions.listConsumerWrapper.Count);
        }
    }
}
