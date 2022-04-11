namespace Reactive.Kafka
{
    public sealed class ProducerWrapper
    {
        private readonly ILogger _logger;

        public ProducerWrapper(ILoggerFactory loggerFactory, IProducer<string, string> producer)
        {
            _logger = loggerFactory.CreateLogger("Reactive.Kafka.Producer");

            if (_logger.IsEnabled(LogLevel.Information))
                _logger.LogInformation("Creating producer {ProducerName}", producer.Name);

            Producer = producer;
        }

        public IProducer<string, string> Producer { get; set; }

        public void OnProduce(string topic, Message<string, string> message)
        {
            if (_logger.IsEnabled(LogLevel.Debug))
                _logger.LogDebug("Inserting message '{MessageValue}' on topic '{TopicName}' synchronous.", message.Value, topic);

            Producer.Produce(topic, message);
        }

        public async Task<DeliveryResult<string, string>> OnProduceAsync(string topic, Message<string, string> message)
        {
            if (_logger.IsEnabled(LogLevel.Debug))
                _logger.LogDebug("Inserting message '{MessageValue}' on topic '{TopicName}' asynchronous.", message.Value, topic);

            return await Producer.ProduceAsync(topic, message);
        }
    }
}
