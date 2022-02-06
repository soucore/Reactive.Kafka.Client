using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Reactive.Kafka.Exceptions;
using Reactive.Kafka.Tests.Types;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Reactive.Kafka.Tests
{
    public class ConsumerTest
    {
        private readonly IConsumer<string, string> _consumer
            = new ConsumerBuilder<string, string>(new ConsumerConfig
            {
                BootstrapServers = "localhost:9092",
                GroupId = Guid.NewGuid().ToString()
            }).Build();

        private readonly ILoggerFactory _loggerFactory = new LoggerFactory();

        [Theory]
        [InlineData("Reactive", "[ Reactive ]")]
        [InlineData("Consumer", "[ Consumer ]")]
        public void OnBeforeSerializationEvent(string rawMessage, string expectedMessage)
        {
            // Arrange
            var consumerWrapper = new ConsumerWrapper<string>(_loggerFactory, _consumer);
            var beforeSerialization = "";
            var kafkaMessage = new Message<string, string> { Key = "", Value = rawMessage };

            // Act
            consumerWrapper.OnBeforeSerialization +=
                rawMessage =>
                {
                    beforeSerialization = $"[ {rawMessage} ]";
                    return beforeSerialization;
                };

            consumerWrapper.ConvertMessage(kafkaMessage);

            // Assert
            Assert.NotEmpty(beforeSerialization);
            Assert.Equal(expectedMessage, beforeSerialization);
        }

        [Fact]
        public void OnAfterSerializationEvent()
        {
            // Arrange
            var consumerWrapper = new ConsumerWrapper<MessageTest>(_loggerFactory, _consumer);
            var kafkaMessage = new Message<string, string> { Key = "", Value = @"{""Id"":1,""Name"":""Paul""}" };
            var expectedMessage = new MessageTest { Id = 1, Name = "John" };

            MessageTest afterSerialization = null;

            // Act
            consumerWrapper.OnAfterSerialization +=
                message =>
                {
                    message.Name = "John";
                    afterSerialization = message;
                    return afterSerialization;
                };

            consumerWrapper.ConvertMessage(kafkaMessage);

            // Assert
            Assert.Equal(expectedMessage.Id, afterSerialization.Id);
            Assert.Equal(expectedMessage.Name, afterSerialization.Name);
        }

        [Fact]
        public async Task UnsuccessfulMessageConversionFromStringToObject()
        {
            // Arrange
            var consumerWrapper = new ConsumerWrapper<MessageTest>(_loggerFactory, _consumer);
            var kafkaMessage = new Message<string, string> { Key = "", Value = "I can't be converted." };

            // Act
            Task action()
            {
                consumerWrapper.ConvertMessage(kafkaMessage);
                return Task.CompletedTask;
            }

            // Assert
            await Assert.ThrowsAsync<KafkaConsumerException>(action);
        }

        [Fact]
        public async Task UnsuccessfulMessageConversionFromStringToInteger()
        {
            // Arrange
            var consumerWrapper = new ConsumerWrapper<int>(_loggerFactory, _consumer);
            var kafkaMessage = new Message<string, string> { Key = "", Value = "I can't be converted." };

            // Act
            Task action()
            {
                consumerWrapper.ConvertMessage(kafkaMessage);
                return Task.CompletedTask;
            }

            // Assert
            await Assert.ThrowsAsync<KafkaConsumerException>(action);
        }

        [Theory]
        [InlineData(@"{""Id"":1,""Name"":""John""}", 1, "John")]
        [InlineData(@"{""Id"":2,""Name"":""Rafael""}", 2, "Rafael")]
        public void OnConsumeEvent(string rawMessage, int expectedId, string expectedName)
        {
            // Arrange
            var consumerWrapper = new ConsumerWrapper<MessageTest>(_loggerFactory, _consumer);
            var kafkaMessage = new Message<string, string> { Key = "", Value = rawMessage };

            int? id = null;
            string name = null;

            // Act
            consumerWrapper.OnConsume +=
                (consumerMessage, commit) =>
                {
                    id = consumerMessage.Message.Id;
                    name = consumerMessage.Message.Name;

                    return Task.CompletedTask;
                };

            consumerWrapper.ConvertMessage(kafkaMessage);

            // Assert
            Assert.NotNull(id);
            Assert.NotNull(name);
            Assert.Equal(expectedId, id);
            Assert.Equal(expectedName, name);
        }
    }
}
