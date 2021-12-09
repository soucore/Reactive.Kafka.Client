using System;
using Xunit;
using FluentAssertions;

namespace Reactive.Kafka.Client.UnitTests
{
    public class FluentKafkaClientTests
    {
        [Fact(Skip="Verify best way to turn on and turn off assemblies")]
        public void RunReactiveKafkaConsumerWithoutConfiguration()
        {
            //Act
            Action action = () => ReactiveKafkaClient.Run(client => client);

            //Assert
            action.Should()
                .Throw<NotImplementedException>()
                .WithMessage("Couldn't find a consumer with 'ReactiveKafkaConsumerAttribute'");
        }
        
    }
}