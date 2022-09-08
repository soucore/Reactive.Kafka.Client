namespace Reactive.Kafka.Interfaces;

public interface IKafkaAdmin
{
    TopicMetadata GetTopicMetadata(IAdminClient adminClient, string topic);
    int Partitions(KafkaConfiguration configuration);
    Metadata GetMetadata(IAdminClient adminClient);
}
