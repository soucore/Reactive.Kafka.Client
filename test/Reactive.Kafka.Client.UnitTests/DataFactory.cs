using AutoFixture;

namespace Reactive.Kafka.Client.UnitTests
{
    public static class DataFactory
    {
        private static Fixture _fixture = new Fixture();

        public static T Create<T>() => _fixture.Create<T>();
    }
}