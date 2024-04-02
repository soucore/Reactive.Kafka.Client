namespace Reactive.Kafka.Interfaces;

public interface IKafkaConsumer<T> : IKafkaConsumer
{
    Task OnConsume(ConsumerMessage<T> consumerMessage, ConsumerContext context, CancellationToken cancellationToken);
}

public interface IKafkaConsumer;
