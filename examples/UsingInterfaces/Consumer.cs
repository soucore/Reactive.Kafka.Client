using Confluent.Kafka;
using Reactive.Kafka;
using Reactive.Kafka.Interfaces;

namespace UsingInterfaces;

public class Consumer : IKafkaConsumer<string>, IKafkaConsumerError, IKafkaConsumerBuilder
{
    private readonly ILogger<Consumer> _logger;

    public Consumer(ILogger<Consumer> logger)
    {
        _logger = logger;
    }

    public async Task OnConsume(ConsumerMessage<string> consumerMessage, ConsumerContext context, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Consumer Name: {ConsumerName}", context.Consumer.Name);
        _logger.LogInformation("Topic:         {Topic}", context.ConsumeResult.Topic);
        _logger.LogInformation("Partition:     {Partition}", context.ConsumeResult.TopicPartition.Partition.Value);
        _logger.LogInformation("Thread:        {Thread}", Environment.CurrentManagedThreadId);
        _logger.LogInformation("Message:       {Message}", consumerMessage.Value);

        await Task.CompletedTask;
    }

    public async Task OnConsumeError(ConsumerContext context)
    {
        _logger.LogError("Ops! Something is wrong.");
        _logger.LogError("Exception: {Exception}", context.Exception);

        await Task.CompletedTask;
    }

    public void OnConsumerBuilder(ConsumerBuilder<string, string> builder)
    {
        builder.SetLogHandler((consumer, log) =>
        {
            _logger.LogDebug("{LogMessage}", log.Message);
        });
    }
}
