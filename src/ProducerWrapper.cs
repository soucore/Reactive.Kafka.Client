namespace Reactive.Kafka;

internal sealed class ProducerWrapper : IProducerWrapper
{
    public ProducerWrapper(ILoggerFactory loggerFactory, IProducer<string, string> producer)
    {
        ProducerLogger = new(loggerFactory.CreateLogger("Reactive.Kafka.Producer"));
        ProducerLogger.LogInformation("Creating producer {ProducerName}", producer.Name);

        Producer = producer;
    }

    #region Properties
    public IProducer<string, string> Producer { get; }
    public LoggerHelper ProducerLogger { get; }
    #endregion

    public void OnProduce(string topic, Message<string, string> message)
    {
        ProducerLogger.LogDebug(
            "Inserting message '{MessageValue}' on topic '{TopicName}' synchronous.", message.Value, topic);

        using var activity = ActivityHelper.CreateProducerActivity(topic, message, Producer.Name);

        try
        {
            using var _ = MeterHelper.RecordProducerPublishDuration(topic);
            Producer.Produce(topic, message);
        }
        catch (Exception ex)
        {
            activity.SetError(ex);
        }
    }

    public async Task<DeliveryResult<string, string>> OnProduceAsync(string topic, Message<string, string> message)
    {
        ProducerLogger.LogDebug(
            "Inserting message '{MessageValue}' on topic '{TopicName}' asynchronous.", message.Value, topic);

        using var activity = ActivityHelper.CreateProducerActivity(topic, message, Producer.Name);

        try
        {
            DeliveryResult<string, string> deliveryResult;

            using (MeterHelper.RecordProducerPublishDuration(topic))
            {
                deliveryResult = await Producer.ProduceAsync(topic, message);
            }

            ActivityHelper
                .SetPartitionOffsetTags(activity, deliveryResult.TopicPartitionOffset);

            return deliveryResult;
        }
        catch (Exception ex)
        {
            activity.SetError(ex);
            return default;
        }
    }
}
