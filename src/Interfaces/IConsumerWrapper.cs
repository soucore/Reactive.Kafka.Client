namespace Reactive.Kafka.Interfaces;

public interface IConsumerWrapper<T> : IConsumerWrapper;

public interface IConsumerWrapper
{
    IConsumer<string, string> Consumer { get; }
    KafkaConfiguration Configuration { get; }
    DateTime LastConsume { get; }
    Task ConsumerStart(TaskCompletionSource<object> taskCompletionSource, CancellationToken stoppingToken);
}
