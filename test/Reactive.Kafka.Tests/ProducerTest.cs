using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Reactive.Kafka.Tests
{
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
    }
}
