namespace Reactive.Kafka
{
    public class KafkaAdmin : IKafkaAdmin
    {
        private readonly ILogger _logger;

        public KafkaAdmin(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger("Reactive.Kafka.Admin");
        }

        public IAdminClient AdminClient { get; set; }

        public Metadata GetMetadata()
        {
            try
            {
                return AdminClient.GetMetadata(TimeSpan.FromSeconds(20));
            }
            catch (Exception ex)
            {
                if (_logger.IsEnabled(LogLevel.Error))
                {
                    _logger.LogError("Unable to obtain kafka metadata from admin.");
                    _logger.LogError("{ErrorMessage}", ex.Message);
                }

                return default;
            }
        }

        public Dictionary<string, int> PartitionsDiscovery()
        {
            Metadata metadata = GetMetadata();
            return PartitionsDiscovery(metadata);
        }

        public Dictionary<string, int> PartitionsDiscovery(Metadata metadata)
        {
            Dictionary<string, int> dict = new();

            metadata?.Topics.ForEach(topicMetadata =>
            {
                if (dict.ContainsKey(topicMetadata.Topic))
                    dict[topicMetadata.Topic] = topicMetadata.Partitions.Count;
                else
                    dict.Add(topicMetadata.Topic, topicMetadata.Partitions.Count);
            });

            return dict;
        }

        public static KafkaAdmin CreateInstance(IServiceProvider provider, bool isTest)
        {
            var kafkaConfiguration = provider.GetRequiredService<KafkaConfiguration>();

            var kafkaAdmin = (KafkaAdmin)ActivatorUtilities
                .CreateInstance(provider, typeof(KafkaAdmin), Array.Empty<object>());

            if (!isTest)
            {
                kafkaAdmin.AdminClient = new AdminClientBuilder(new AdminClientConfig
                {
                    BootstrapServers = kafkaConfiguration.ConsumerConfig.BootstrapServers
                }).Build();
            }

            return kafkaAdmin;
        }
    }
}
