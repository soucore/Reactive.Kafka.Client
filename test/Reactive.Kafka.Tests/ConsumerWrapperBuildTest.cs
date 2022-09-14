using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Hosting;
using Reactive.Kafka.Configurations;

namespace Reactive.Kafka.Tests
{
    public class ConsumerWrapperBuildTest
    {
        private readonly IServiceProvider provider;
        private readonly KafkaConfiguration configuration = new();

        public ConsumerWrapperBuildTest()
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
        public void ShouldEmitEventOnInit()
        {
            // Arrange
            configuration.Topic = "test-topic";
            configuration.ConsumerConfig.BootstrapServers = "localhost:9092";
            configuration.ConsumerConfig.GroupId = "test-group";
            var consumerObject = new Consumer4OnInit();

            // Act
            _ = new ConsumerWrapperBuilder<Consumer4OnInit, string>(consumerObject, configuration, provider)
                .Build();

            //Assert
            Assert.True(consumerObject.IsInit);
        }
    }
}
