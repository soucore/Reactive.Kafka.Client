using Confluent.Kafka;
using Reactive.Kafka;

namespace ConsumerPerPartition;

public class Consumer2 : ConsumerBase<Message>
{
    private readonly ILogger<Consumer2> _logger;

    public Consumer2(ILogger<Consumer2> logger)
    {
        _logger = logger;
    }

    public override async Task OnConsume(ConsumerMessage<Message> consumerMessage, ConsumerContext context, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Consumer Name: {ConsumerName}", context.Consumer.Name);
        _logger.LogInformation("Topic:         {Topic}", context.ConsumeResult.Topic);
        _logger.LogInformation("Partition:     {Partition}", context.ConsumeResult.TopicPartition.Partition.Value);
        _logger.LogInformation("Thread:        {Thread}", Environment.CurrentManagedThreadId);
        _logger.LogInformation("Message:       {Message}", consumerMessage.Value);

        context.Commit();

        await Task.CompletedTask;
    }

    public override void OnConsumerConfiguration(ConsumerConfig configuration)
    {
        configuration.EnableAutoCommit = false;
        configuration.AutoCommitIntervalMs = 0;
        configuration.AutoOffsetReset = AutoOffsetReset.Latest;
    }
}
