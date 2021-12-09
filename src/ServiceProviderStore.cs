using Microsoft.Extensions.DependencyInjection;

namespace Reactive.Kafka
{
    public static class ServiceProviderStore
    {
        public static ServiceProvider? ServiceProvider { get; set; }
    }
}