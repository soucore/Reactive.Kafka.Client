namespace Reactive.Kafka.Interfaces;

public interface IConsumerWrapper<T> : IConsumerWrapper
{
    void SuccessfulConversion(ConsumerMessage<T> consumerMessage, ConsumerContext context);
}

public interface IConsumerWrapper
{
    IConsumer<string, string> Consumer { get; }
    KafkaConfiguration Configuration { get; }
    DateTime LastConsume { get; }
    Task ConsumerStart(TaskCompletionSource<object> taskCompletionSource, CancellationToken stoppingToken);
    ConsumeResult<string, string> ConsumeMessage(CancellationToken stoppingToken);
    void UnsuccessfulConversion(Message<string, string> kafkaMessage);
}
