namespace Reactive.Kafka.Interfaces;

public interface IKafkaConsumerError
{
    Task OnConsumeError(ConsumerContext context);
}
