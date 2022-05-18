using Reactive.Kafka.Configurations;

namespace Reactive.Kafka.Tests
{
    public class KafkaAdminTest
    {
        private readonly IServiceProvider provider;
        private readonly Fixture fixture = new();

        public KafkaAdminTest()
        {
            var services = new ServiceCollection();

            var loggerFactorySub = new Mock<ILoggerFactory>();
            var loggerStub = new Mock<ILogger>();

            loggerFactorySub
                .Setup(x => x.CreateLogger(It.IsAny<string>()))
                .Returns(loggerStub.Object);

            services.AddSingleton(loggerFactorySub.Object);
            services.AddSingleton(new KafkaConfiguration());

            provider = services.BuildServiceProvider();
        }

        [Fact]
        public void ShouldGetSuccessfulMetadata()
        {
            // Arrange
            var (kafkaAdmin, metadata, adminClientStub) = Setup();

            adminClientStub
                .Setup(x => x.GetMetadata(It.IsAny<TimeSpan>()))
                .Returns(metadata);

            kafkaAdmin.AdminClient = adminClientStub.Object;

            // Act
            Metadata sut = kafkaAdmin.GetMetadata();

            // Assert
            sut.Should().Be(metadata);
        }

        [Fact]
        public void ShouldGetUnsuccessfulMetadata()
        {
            // Arrange
            var (kafkaAdmin, metadata, adminClientStub) = Setup();


            adminClientStub
                .Setup(x => x.GetMetadata(It.IsAny<TimeSpan>()))
                .Throws<Exception>();

            kafkaAdmin.AdminClient = adminClientStub.Object;

            // Act
            var sut = kafkaAdmin.GetMetadata();

            // Assert
            sut.Should().BeNull();
        }

        [Fact]
        public void ShouldCorrectlyMapTopicsAndTheirPartitionsFromMetadata()
        {
            // Arrange
            var kafkaAdmin = KafkaAdmin.CreateInstance(provider, isTest: true);
            var metadata = new Metadata(null, fixture.Create<List<TopicMetadata>>(), 0, null);

            // Act
            var sut = kafkaAdmin.PartitionsDiscovery(metadata);

            // Assert
            sut.Should().HaveCountGreaterThan(0);
        }

        [Fact]
        public void ShouldCorrectlyMapTopicsAndTheirPartitions()
        {
            // Arrange
            var (kafkaAdmin, metadata, adminClientStub) = Setup();

            adminClientStub
                .Setup(x => x.GetMetadata(It.IsAny<TimeSpan>()))
                .Returns(metadata);

            kafkaAdmin.AdminClient = adminClientStub.Object;

            // Act
            var sut = kafkaAdmin.PartitionsDiscovery();

            // Assert
            sut.Should().HaveCountGreaterThan(0);
        }

        private (KafkaAdmin, Metadata, Mock<IAdminClient>) Setup()
        {
            var kafkaAdmin = KafkaAdmin.CreateInstance(provider, isTest: true);
            var metadata = new Metadata(null, fixture.Create<List<TopicMetadata>>(), 0, null);
            var adminClientStub = new Mock<IAdminClient>();

            return (kafkaAdmin, metadata, adminClientStub);
        }
    }
}
