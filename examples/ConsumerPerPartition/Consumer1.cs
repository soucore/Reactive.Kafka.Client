using Reactive.Kafka;

namespace ConsumerPerPartition;

public class Consumer1 : ConsumerBase<Message>
{
    private readonly ILogger<Consumer1> _logger;

    public Consumer1(ILogger<Consumer1> logger)
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

        await Task.CompletedTask;
    }
}
