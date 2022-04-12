namespace Reactive.Kafka.Tests
{
    public class KafkaReflectionTest
    {
        private readonly IServiceProvider provider;
        private readonly Fixture fixture = new();

        public KafkaReflectionTest()
        {
            // Setup
            var services = new ServiceCollection();

            services.AddSingleton<ILoggerFactory, LoggerFactory>();
            services.AddSingleton<IList<IConsumerWrapper>, List<IConsumerWrapper>>();
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
        public void ShouldCreateAppropriateConsumerWrapper()
        {
            // Arrange
            var sut = KafkaReflection.CreateInstance(provider, typeof(Consumer1), isTest: true);

            // Act
            sut.GenConsumerWrapperType();

            // Assert
            sut.consumerWrapperType.Should().NotBeNull();
            sut.consumerWrapperType.Should().BeAssignableTo<ConsumerWrapper<string>>();
        }

        [Fact]
        public void ShouldBuildIncorrectlyConsumer()
        {
            // Arrange
            var consumerInstance = new Consumer1();
            var kafkaReflection = KafkaReflection.CreateInstance(provider, consumerInstance.GetType(), isTest: true);

            // Act
            var sut = kafkaReflection.BuildConsumer(consumerInstance);

            // Assert
            sut.Should().BeNull();
        }

        [Fact]
        public void ShouldBuildAppropriateConsumer()
        {
            // Arrange
            var consumerInstance = new Consumer2();
            var kafkaReflection = KafkaReflection.CreateInstance(provider, consumerInstance.GetType(), isTest: true);

            // Act
            var sut = kafkaReflection.BuildConsumer(consumerInstance);

            // Assert
            sut.Should().NotBeNull();
            sut.Should().BeAssignableTo<IConsumer<string, string>>();
            sut.Subscription.Should().HaveCount(1);
            sut.Subscription.Should().Contain("test-topic");
        }

        [Fact]
        public void ShouldBuildIncorrectlyProducer()
        {
            // Arrange
            var consumerInstance = new Consumer1();
            var kafkaReflection = KafkaReflection.CreateInstance(provider, consumerInstance.GetType(), isTest: true);

            // Act
            var sut = kafkaReflection.BuildProducer(consumerInstance);

            // Assert
            sut.Should().BeNull();
        }

        [Fact]
        public void ShouldBuildAppropriateProducer()
        {
            // Arrange
            var consumerInstance = new Consumer3();
            var kafkaReflection = KafkaReflection.CreateInstance(provider, consumerInstance.GetType(), isTest: true);

            // Act
            var sut = kafkaReflection.BuildProducer(consumerInstance);

            // Assert
            sut.Should().NotBeNull();
            sut.Should().BeAssignableTo<IProducer<string, string>>();
        }

        [Fact]
        public void ShouldBuildIncorrectlyConsumerAndProducer()
        {
            // Arrange
            var consumerInstance = new Consumer1();
            var kafkaReflection = KafkaReflection.CreateInstance(provider, consumerInstance.GetType(), isTest: true);

            // Act
            (IConsumer<string, string> Consumer, IProducer<string, string> Producer) = kafkaReflection.Build();

            // Assert
            Consumer.Should().BeNull();
            Producer.Should().BeNull();
        }

        [Fact]
        public void ShouldBuildAppropriateConsumerAndProducer()
        {
            // Arrange
            var consumerInstance = new Consumer3();
            var kafkaReflection = KafkaReflection.CreateInstance(provider, consumerInstance.GetType(), isTest: true);

            // Act
            (IConsumer<string, string> Consumer, IProducer<string, string> Producer) = kafkaReflection.Build();

            // Assert
            Consumer.Should().NotBeNull();
            Consumer.Should().BeAssignableTo<IConsumer<string, string>>();
            Producer.Should().NotBeNull();
            Producer.Should().BeAssignableTo<IProducer<string, string>>();
        }

        [Fact]
        public void ShouldCorrectlyBindEvents()
        {
            // Arrange
            var consumerStub = new Mock<IConsumer<string, string>>();
            var loggerStub = new Mock<ILogger>();
            var loggerFactoryStub = new Mock<ILoggerFactory>();

            loggerStub.Setup(x => x.IsEnabled(It.IsAny<LogLevel>())).Returns(false);
            loggerFactoryStub.Setup(x => x.CreateLogger(It.IsAny<string>())).Returns(loggerStub.Object);

            var consumerInstanceMock = new Mock<Consumer3>(Array.Empty<object>());
            var consumerWrapperInstance = new ConsumerWrapper<string>(loggerFactoryStub.Object, consumerStub.Object);

            // Act
            KafkaReflection.BindConsumerEvents(consumerInstanceMock.Object, consumerWrapperInstance);

            consumerWrapperInstance.ConvertMessage(fixture.Create<Message<string, string>>());

            // Assert
            consumerInstanceMock.Verify(x => x.OnBeforeSerialization(It.IsAny<string>()), Times.Once);
            consumerInstanceMock.Verify(x => x.OnAfterSerialization(It.IsAny<string>()), Times.Once);
            consumerInstanceMock.Verify(x => x.OnConsume(It.IsAny<ConsumerMessage<string>>(), It.IsAny<Commit>()), Times.Once);
        }
    }
}
