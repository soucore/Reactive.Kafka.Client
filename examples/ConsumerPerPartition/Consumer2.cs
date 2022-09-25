using Confluent.Kafka;
using Reactive.Kafka;
using Reactive.Kafka.Exceptions;

namespace ConsumerPerPartition;

public class Consumer2 : ConsumerBase<string>
{
    private readonly ILogger<Consumer2> _logger;

    public Consumer2(ILogger<Consumer2> logger)
    {
        _logger = logger;
    }

    public override Task OnConsume(ConsumerMessage<string> consumerMessage, ConsumerContext context)
    {
        _logger.LogInformation("[{MemberId}] {Message}", context.Consumer.Name, consumerMessage.Value);

        return Task.CompletedTask;
    }

    public override Task OnConsumeError(ConsumerContext context)
    {
        if (context.Exception is KafkaSerializationException)
        {
            _logger.LogError("{ErrorMessage}", context.Exception.Message);
            _logger.LogError("{Message}", context.RawMessage);
        }

        return Task.CompletedTask;
    }

    public override void OnConsumerBuilder(ConsumerBuilder<string, string> builder)
    {
        builder.SetLogHandler((consumer, logMessage) => { });
    }

    public override void OnConsumerConfiguration(ConsumerConfig configuration)
    {
        configuration.SessionTimeoutMs = 10 * 1000;
        configuration.Debug = "protocol";
    }

    public override void OnProducerConfiguration(ProducerConfig configuration)
    {
        configuration.BootstrapServers = "localhost:9092";
        configuration.Acks = Acks.None;
    }
}
