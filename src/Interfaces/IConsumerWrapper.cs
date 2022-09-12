namespace Reactive.Kafka.Interfaces;

public interface IConsumerWrapper<T> : IConsumerWrapper
{
    (bool, T) ConvertMessage(Message<string, string> kafkaMessage);
    void SuccessfulConversion(string key, T message);
}

public interface IConsumerWrapper
{
    IConsumer<string, string> Consumer { get; }
    KafkaConfiguration Configuration { get; }
    DateTime LastConsume { get; }
    Task ConsumerStart();
    Message<string, string> ConsumeMessage();
    void UnsuccessfulConversion(Message<string, string> kafkaMessage);
}
