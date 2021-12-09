using System;
using System.Collections.Generic;
using Reactive.Kafka.Attributes;
using Xunit;
using FluentAssertions;

namespace Reactive.Kafka.Client.UnitTests
{
    public class ConsumerListenerAttributeTests
    {
        private ConsumerListenerAttribute _sut;

        public ConsumerListenerAttributeTests() {
            var topic = DataFactory.Create<string>();
            _sut = new ConsumerListenerAttribute(topic);
        }

        [Fact]
        public void AttributeUsageShouldOnlyBeValidOnMethod()
        {   
            //Act
            var result = _sut.GetType()
                .GetCustomAttributes(typeof(AttributeUsageAttribute), false)
                as IList<AttributeUsageAttribute>;

            //Assert
            result.Should().ContainSingle()
                .And.OnlyContain(attr => attr.ValidOn == AttributeTargets.Method);
        }

        [Theory]
        [InlineData("topic")]
        [InlineData("topic1", "topic2", "topic3")]
        public void ListenerAttributeShouldBeCreatedWithOneOrMoreTopics(params string[] topics)
        {   
            //Arrange
            _sut = new ConsumerListenerAttribute(topics);

            //Act
            var result = _sut.Topics;

            //Assert
            result.Should().HaveSameCount(topics);
            result.Should().Equals(topics);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void ListenerAttributeShouldNotBeCreatedWithoutTopics(params string[] topics)
        {
            //Act
            var action = () => new ConsumerListenerAttribute(topics);

            //Assert
            action.Should().Throw<Exception>()
                .WithMessage("Cannot create Consumer Listener without topics");
        }
    }
}