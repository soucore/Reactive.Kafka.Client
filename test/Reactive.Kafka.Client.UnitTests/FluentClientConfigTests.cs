using Xunit;
using FluentAssertions;
using Confluent.Kafka;

namespace Reactive.Kafka.Client.UnitTests {
    public class FluentClientConfigTests
    {
        private FluentClientConfig _sut = new FluentClientConfig();

        public FluentClientConfigTests()
        {
            _sut = new FluentClientConfig();
        }

        [Fact]
        public void ConsumerConfigShouldBeNullByDefault()
        {
            //Act
            var result = _sut.ConsumerConfig;

            //Assert
            result.Should().BeNull();
        }

        [Fact]
        public void IsUseConsumerShouldBeFalseByDefault()
        {
            //Act
            var result = _sut.IsUseConsumer;

            //Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void ProducerConfigShouldBeNullByDefault()
        {
            //Act
            var result = _sut.ProducerConfig;

            //Assert
            result.Should().BeNull();
        }

        [Fact]
        public void IsUseProducerShouldBeFalseByDefault()
        {
            //Act
            var result = _sut.IsUseProducer;

            //Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void EnableMultithreadShouldBeFalseByDefault()
        {
            //Act
            var result = _sut.IsConsumerMultiThread;

            //Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void ConsumerConfigShouldBeUpdatedWithSuccess()
        {
            //Arrange
            var config = DataFactory.Create<ConsumerConfig>();
            _sut.UseConsumer(config);

            //Act
            var result = new { _sut.ConsumerConfig, _sut.IsUseConsumer };

            //Assert
            result.ConsumerConfig.Should().NotBeNull();
            result.ConsumerConfig.Should().Equals(config);
            result.IsUseConsumer.Should().BeTrue();
        }

        [Fact]
        public void ProducerConfigShouldBeUpdatedWithSuccess()
        {
            //Arrange
            var config = DataFactory.Create<ProducerConfig>();
            _sut.UseProducer(config);

            //Act
            var result = new { _sut.ProducerConfig, _sut.IsUseProducer };

            //Assert
            result.ProducerConfig.Should().NotBeNull();
            result.ProducerConfig.Should().Equals(config);
            result.IsUseProducer.Should().BeTrue();
        }

        [Fact]
        public void EnableConsumerMultithreadShouldBeUpdatedWithSuccess()
        {
            //Arrange
            _sut.EnableConsumerMultiThread();

            //Act
            var result = _sut.IsConsumerMultiThread;

            //Assert
            result.Should().BeTrue();
        }
    }
}