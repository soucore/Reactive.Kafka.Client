namespace Reactive.Kafka.Interfaces
{
    public interface IKafkaAdmin
    {
        Metadata GetMetadata();
        Dictionary<string, int> PartitionsDiscovery();
        Dictionary<string, int> PartitionsDiscovery(Metadata metadata);
    }
}
