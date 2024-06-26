﻿namespace Reactive.Kafka;

public sealed class KafkaAdmin(ILoggerFactory loggerFactory) : IKafkaAdmin
{
    private readonly ILogger _logger = loggerFactory.CreateLogger("Reactive.Kafka.Admin");

    public int Partitions(KafkaConfiguration configuration)
    {
        using var adminClient = new AdminClientBuilder(new AdminClientConfig
        {
            BootstrapServers = configuration.ConsumerConfig.BootstrapServers
        }).Build();

        var topicMetadata = GetTopicMetadata(adminClient, configuration.Topic);
        var topicPartitions = topicMetadata?.Partitions.Count ?? 1;

        if (topicMetadata is null)
        {
            if (_logger.IsEnabled(LogLevel.Debug))
                _logger.LogDebug("Unable to obtain metadata for topic '{Topic}'. Assuming 1 partition.", configuration.Topic);
        }
        else
        {
            if (_logger.IsEnabled(LogLevel.Debug))
                _logger.LogDebug("Topic '{Topic}' has {Partitions} partitions.", configuration.Topic, topicPartitions);
        }

        return topicPartitions;
    }

    public TopicMetadata GetTopicMetadata(IAdminClient adminClient, string topic)
    {
        return GetMetadata(adminClient)?.Topics.FirstOrDefault(tm => tm.Topic == topic);
    }

    public Metadata GetMetadata(IAdminClient adminClient)
    {
        try
        {
            return adminClient.GetMetadata(TimeSpan.FromSeconds(20));
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
}
