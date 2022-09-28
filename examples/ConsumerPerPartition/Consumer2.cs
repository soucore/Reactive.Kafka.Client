using Confluent.Kafka;
using Reactive.Kafka;
using Reactive.Kafka.Errors;

namespace ConsumerPerPartition
{
    public class Consumer2 : ConsumerBase<string>
    {
        private readonly ILogger<Consumer2> _logger;
        private readonly Random random = new Random();

        private IDisposable correlationId = null;

        public Consumer2(ILogger<Consumer2> logger)
        {
            _logger = logger;
        }

        public override async Task OnConsume(ConsumerMessage<string> consumerMessage, Commit commit)
        {
            _logger.LogInformation("[{Thread}] {Message}", Environment.CurrentManagedThreadId, consumerMessage.Message);
            await Task.Delay(90 * 1000);

            commit();
        }

        public override Task OnConsumeError(KafkaConsumerError consumerError, Commit commit)
        {
            _logger.LogError(consumerError.Exception.InnerException, "An error has occured.");
            commit();
            return Task.CompletedTask;
        }

        public override void OnConsumerConfiguration(ConsumerConfig configuration)
        {
            configuration.EnableAutoCommit = false;
            configuration.AutoCommitIntervalMs = 0;
            configuration.SessionTimeoutMs = 10 * 1000;
            configuration.AutoOffsetReset = AutoOffsetReset.Latest;
            configuration.MaxPollIntervalMs = 60 * 1000;
        }

        public override void OnProducerConfiguration(ProducerConfig configuration)
        {
            configuration.BootstrapServers = "localhost:9092";
            configuration.Acks = Acks.None;
        }
    }
}
