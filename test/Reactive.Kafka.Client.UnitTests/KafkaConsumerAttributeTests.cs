using System;
using System.Collections.Generic;
using Reactive.Kafka.Attributes;
using Xunit;
using FluentAssertions;

namespace Reactive.Kafka.Client.UnitTests
{
    public class KafkaConsumerAttributeTests
    {
        private ReactiveKafkaConsumerAttribute _sut;

        public KafkaConsumerAttributeTests() {
            _sut = new ReactiveKafkaConsumerAttribute();
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
                .And.OnlyContain(attr => attr.ValidOn == AttributeTargets.Class);
        }
    }
}