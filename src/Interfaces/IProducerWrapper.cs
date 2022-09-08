namespace Reactive.Kafka.Interfaces;

internal interface IProducerWrapper
{
    void OnProduce(string topic, Message<string, string> message);
    Task<DeliveryResult<string, string>> OnProduceAsync(string topic, Message<string, string> message);
}
