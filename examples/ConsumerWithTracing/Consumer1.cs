using Reactive.Kafka;

namespace ConsumerWithTracing;

internal class Consumer1 : ConsumerBase<string>
{
    private readonly ILogger<Consumer1> _logger;
    private readonly HttpClient _client;

    public Consumer1(ILogger<Consumer1> logger, IHttpClientFactory factory)
    {
        _client = factory.CreateClient("consumer-1");
        _logger = logger;
    }

    public override async Task OnConsume(ConsumerMessage<string> consumerMessage, ConsumerContext context, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Consumer Name: {ConsumerName}", context.Consumer.Name);
        _logger.LogInformation("Topic:         {Topic}", context.ConsumeResult.Topic);
        _logger.LogInformation("Partition:     {Partition}", context.ConsumeResult.TopicPartition.Partition.Value);
        _logger.LogInformation("Thread:        {Thread}", Environment.CurrentManagedThreadId);
        _logger.LogInformation("Message:       {Message}", consumerMessage.Value);

        // Calling a fake API to test nested traces.
        await _client.GetAsync("/products", cancellationToken);
    }
}
