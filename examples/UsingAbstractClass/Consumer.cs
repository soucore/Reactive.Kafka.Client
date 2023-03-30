using Reactive.Kafka;

namespace UsingAbstractClass;

public class Consumer : ConsumerBase<string>
{
    private readonly ILogger<Consumer> _logger;

    public Consumer(ILogger<Consumer> logger)
    {
        _logger = logger;
    }

    public override async Task OnConsume(ConsumerMessage<string> consumerMessage, ConsumerContext context)
    {
        _logger.LogInformation("Consumer Name: {ConsumerName}", context.Consumer.Name);
        _logger.LogInformation("Topic:         {Topic}", context.ConsumeResult.Topic);
        _logger.LogInformation("Partition:     {Partition}", context.ConsumeResult.TopicPartition.Partition.Value);
        _logger.LogInformation("Thread:        {Thread}", Environment.CurrentManagedThreadId);
        _logger.LogInformation("Message:       {Message}", consumerMessage.Value);

        await Task.CompletedTask;
    }

    public override async Task OnConsumeError(ConsumerContext context)
    {
        _logger.LogError("Ops! Something is wrong!");
        _logger.LogError("Exception: {Exception}", context.Exception);

        await Task.CompletedTask;
    }
}