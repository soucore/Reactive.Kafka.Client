using Microsoft.Extensions.DependencyInjection;
using Reactive.Kafka.Extensions;
using Reactive.Kafka.Tests.Types;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Reactive.Kafka.Tests
{
    public class ServiceCollectionExtensionsTest
    {
        [Fact]
        public async Task AddReactiveKafkaConsumerPerPartitionWithNullParams()
        {
            // Arrange
            IServiceCollection services = new ServiceCollection();

            // Act
            async Task servicesParamIsNull()
            {
                ServiceCollectionExtensions
                    .AddReactiveKafkaConsumerPerPartition<Consumer1>(
                        null, "localhost:9092");
            }

            async Task bootstrapParamIsNull()
            {
                ServiceCollectionExtensions
                    .AddReactiveKafkaConsumerPerPartition<Consumer1>(
                        services, null);
            }

            // Assert
            await Assert.ThrowsAsync<ArgumentNullException>(servicesParamIsNull);
            await Assert.ThrowsAsync<ArgumentNullException>(bootstrapParamIsNull);
        }

        [Fact]
        public async Task AddReactiveKafkaConsumerPerQuantityWithNullParams()
        {
            // Arrange
            IServiceCollection services = new ServiceCollection();

            // Act
            async Task servicesParamIsNull()
            {
                ServiceCollectionExtensions
                    .AddReactiveKafkaConsumerPerQuantity<Consumer1>(
                        null, "localhost:9092", quantity: 0);
            }

            async Task bootstrapParamIsNull()
            {
                ServiceCollectionExtensions
                    .AddReactiveKafkaConsumerPerQuantity<Consumer1>(
                        services, null, quantity: 0);
            }

            // Assert
            await Assert.ThrowsAsync<ArgumentNullException>(servicesParamIsNull);
            await Assert.ThrowsAsync<ArgumentNullException>(bootstrapParamIsNull);
        }

        [Fact]
        public async Task AddReactiveKafkaConsumerWithNullParams()
        {
            // Arrange
            IServiceCollection services = new ServiceCollection();

            // Act
            async Task servicesParamIsNull()
            {
                ServiceCollectionExtensions
                    .AddReactiveKafkaConsumer(
                        null, "localhost:9092", groupId: null);
            }

            async Task bootstrapParamIsNull()
            {
                ServiceCollectionExtensions
                    .AddReactiveKafkaConsumer(
                        services, null, groupId: null);
            }

            async Task setupActionIsNull()
            {
                ServiceCollectionExtensions
                    .AddReactiveKafkaConsumer(
                        services, null, assembly: null);
            }

            // Assert
            await Assert.ThrowsAsync<ArgumentNullException>(servicesParamIsNull);
            await Assert.ThrowsAsync<ArgumentNullException>(bootstrapParamIsNull);
            await Assert.ThrowsAsync<ArgumentNullException>(setupActionIsNull);
        }
    }
}
