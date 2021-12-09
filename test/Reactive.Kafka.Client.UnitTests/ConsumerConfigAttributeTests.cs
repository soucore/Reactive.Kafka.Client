using System;
using System.Collections.Generic;
using Reactive.Kafka.Attributes;
using Xunit;
using FluentAssertions;

namespace Reactive.Kafka.Client.UnitTests
{
    public class ConsumerConfigAttributeTests
    {
        private ConsumerConfigAttribute _sut;

        public ConsumerConfigAttributeTests() {
            _sut = new ConsumerConfigAttribute();
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
    }
}