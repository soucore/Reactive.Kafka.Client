using Reactive.Kafka.Extensions;

namespace Reactive.Kafka.Tests.Extensions;

public class ServiceCollectionExtentionsTest
{
    private readonly IServiceCollection services;

    public ServiceCollectionExtentionsTest()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddSingleton<ILoggerFactory, LoggerFactory>();

        services = serviceCollection;
    }

    [Fact]
    public void ShouldInitializeAppropriateServices()
    {
        // Arrange & Act
        services.AddReactiveKafka((provider, configurator) =>
        {
            configurator.AddConsumerPerQuantity<Consumer1, string>("localhost", 2, "test-topic", "test-group");
        });

        // Assert
        services.Should().Contain(sd => sd.ServiceType == typeof(IList<IConsumerWrapper>));
        services.Should().Contain(sd => sd.ServiceType == typeof(IConsumerConfigurator));
    }
}
