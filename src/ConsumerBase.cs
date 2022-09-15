namespace Reactive.Kafka;

public abstract class ConsumerBase<T> : IKafkaConsumer<T>, IKafkaConsumerConfiguration, IKafkaConsumerError, IKafkaSerialization<T>
{
    #region producer events
    public event Action<string, Message<string, string>> OnProduce;
    public event Func<string, Message<string, string>, Task<DeliveryResult<string, string>>> OnProduceAsync;
    #endregion

    public virtual string OnBeforeSerialization(string rawMessage) => rawMessage;
    public virtual T OnAfterSerialization(T message) => message;

    /// <summary>
    /// Method executed immediately after subscribing to the topic and before starting to consume the message.
    /// </summary>
    public virtual void OnReady() { }

    public virtual void OnProducerConfiguration(ProducerConfig configuration) { }
    public virtual void OnConsumerConfiguration(ConsumerConfig configuration) { }
    public virtual void OnConsumerBuilder(ConsumerBuilder<string, string> builder) { }
    public virtual Task OnConsumeError(KafkaConsumerError consumerError, Commit commit)
    {
        return Task.CompletedTask;
    }

    #region Abstract Methods
    public abstract Task OnConsume(ConsumerMessage<T> consumerMessage, Commit commit);
    #endregion

    public void Produce(string topic, string message)
    {
        Produce(topic, new Message<string, string> { Value = message });
    }

    public void Produce(string topic, Message<string, string> message)
    {
        OnProduce?.Invoke(topic, message);
    }

    public async Task<DeliveryResult<string, string>> ProduceAsync(string topic, string message)
    {
        return await ProduceAsync(topic, new Message<string, string> { Value = message });
    }

    public async Task<DeliveryResult<string, string>> ProduceAsync(string topic, Message<string, string> message)
    {
        if (OnProduceAsync is not null)
            return await OnProduceAsync.Invoke(topic, message);

        return await Task.FromResult(default(DeliveryResult<string, string>));
    }
}
