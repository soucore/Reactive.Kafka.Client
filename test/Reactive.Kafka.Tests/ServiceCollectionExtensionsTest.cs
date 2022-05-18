using Reactive.Kafka.Configurations;
using Reactive.Kafka.Extensions;
using System.Threading.Tasks;

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
            Task servicesParamIsNull()
            {
                ServiceCollectionExtensions
                    .AddReactiveKafkaConsumerPerPartition<Consumer1>(
                        null, "localhost:9092");

                return Task.CompletedTask;
            }

            Task bootstrapParamIsNull()
            {
                ServiceCollectionExtensions
                    .AddReactiveKafkaConsumerPerPartition<Consumer1>(
                        services, null);

                return Task.CompletedTask;
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
            Task servicesParamIsNull()
            {
                ServiceCollectionExtensions
                    .AddReactiveKafkaConsumerPerQuantity<Consumer1>(
                        null, "localhost:9092", quantity: 0);

                return Task.CompletedTask;
            }

            Task bootstrapParamIsNull()
            {
                ServiceCollectionExtensions
                    .AddReactiveKafkaConsumerPerQuantity<Consumer1>(
                        services, null, quantity: 0);

                return Task.CompletedTask;
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
            Task servicesParamIsNull()
            {
                ServiceCollectionExtensions
                    .AddReactiveKafkaConsumer(
                        null, "localhost:9092");

                return Task.CompletedTask;
            }

            Task bootstrapParamIsNull()
            {
                ServiceCollectionExtensions
                    .AddReactiveKafkaConsumer(
                        services, null);

                return Task.CompletedTask;
            }

            // Assert
            await Assert.ThrowsAsync<ArgumentNullException>(servicesParamIsNull);
            await Assert.ThrowsAsync<ArgumentNullException>(bootstrapParamIsNull);
        }

        [Fact]
        public void ShoudCountListConsumerWrapper()
        {
            // Arrange
            IServiceCollection services = new ServiceCollection();

            services.AddSingleton<ILoggerFactory, LoggerFactory>();
            services.AddSingleton(ServiceCollectionExtensions.listConsumerWrapper);
            services.AddTransient(provider =>
            {
                var config = new KafkaConfiguration();

                config.ConsumerConfig.BootstrapServers = "localhost:9092";
                config.ConsumerConfig.GroupId = "Group";

                return config;
            });

            IServiceProvider provider = services.BuildServiceProvider();

            // Act
            ServiceCollectionExtensions
                .ApplyConsumerPerQuantity(provider, typeof(Consumer2), 2, test: true);

            // Assert
            ServiceCollectionExtensions
                .listConsumerWrapper
                .Should()
                .HaveCount(2);
        }
    }
}
