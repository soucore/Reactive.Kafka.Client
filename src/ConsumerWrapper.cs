﻿using Convert = Reactive.Kafka.Helpers.Convert;

namespace Reactive.Kafka;

#region Custom Delegates
public delegate List<TopicPartitionOffset> Commit();
public delegate Task EventHandlerAsync<in TMessage>(TMessage e, Commit commit);
#endregion

public sealed class ConsumerWrapper<T> : IConsumerWrapper<T>
{
    #region Events
    public event Func<string, string> OnBeforeSerialization;
    public event Func<T, T> OnAfterSerialization;
    public event EventHandlerAsync<ConsumerMessage<T>> OnConsume;
    public event EventHandlerAsync<KafkaConsumerError> OnConsumeError;
    #endregion

    private readonly ILogger _logger;

    public ConsumerWrapper(ILoggerFactory loggerFactory, IConsumer<string, string> consumer, KafkaConfiguration configuration)
    {
        _logger = loggerFactory.CreateLogger("Reactive.Kafka.Consumer");

        if (_logger.IsEnabled(LogLevel.Information))
            _logger.LogInformation("Creating consumer {ConsumerName}", consumer.Name);

        Consumer = consumer;
        Configuration = configuration;
    }

    public IConsumer<string, string> Consumer { get; }
    public KafkaConfiguration Configuration { get; }
    public DateTime LastConsume { get; private set; } = DateTime.Now;

    public Task ConsumerStart(TaskCompletionSource<object> taskCompletionSource, CancellationToken stoppingToken)
    {
        if (_logger.IsEnabled(LogLevel.Information))
            _logger.LogInformation("Initializing consumer {ConsumerName}", Consumer.Name);

        return Task.Factory.StartNew(() =>
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    Message<string, string> consumedMessage = ConsumeMessage(stoppingToken);

                    if (consumedMessage is null)
                        continue;

                    (bool successfulConversion, T message) = ConvertMessage(consumedMessage);

                    if (successfulConversion)
                    {
                        SuccessfulConversion(consumedMessage.Key, message);
                        continue;
                    }

                    UnsuccessfulConversion(consumedMessage);
                }
                catch (KafkaConsumerException ex)
                {
                    OnConsumeError?.Invoke(new KafkaConsumerError(ex), Consumer.Commit).Wait();
                }
                catch (ConsumeException ex)
                {
                    if (_logger.IsEnabled(LogLevel.Error))
                        _logger.LogError(ex, "Unable to consume messages from consumer {ConsumerName}.", Consumer.Name);

                    OnConsumeError?.Invoke(new KafkaConsumerError(ex), Consumer.Commit).Wait();
                }
                catch (KafkaException ex)
                {
                    if (_logger.IsEnabled(LogLevel.Error))
                        _logger.LogError(ex, "An internal kafka error has occurred.");

                    OnConsumeError?.Invoke(new KafkaConsumerError(ex), Consumer.Commit).Wait();
                }
                catch (Exception ex)
                {
                    if (_logger.IsEnabled(LogLevel.Error))
                        _logger.LogError(ex, "Consumer failed", ex.Message);

                    throw;
                }
            }

            _logger.LogInformation("Stopping consumer {ConsumerName}", Consumer.Name);

            taskCompletionSource?.TrySetResult(null);

        }, stoppingToken, TaskCreationOptions.LongRunning, TaskScheduler.Current);
    }

    public Message<string, string> ConsumeMessage(CancellationToken stoppingToken)
    {
        try
        {
            var result = Consumer.Consume(stoppingToken);

            if (result != null && _logger.IsEnabled(LogLevel.Debug))
                _logger.LogDebug("Message received from partition {Partition}: {MessageValue}", result.Partition.Value, result.Message.Value);

            return result.Message;
        }
        catch (OperationCanceledException)
        {
            return null;
        }
    }

    public (bool, T) ConvertMessage(Message<string, string> kafkaMessage)
    {
        if (OnBeforeSerialization is not null)
            kafkaMessage.Value = OnBeforeSerialization.Invoke(kafkaMessage.Value);

        if (Convert.TrySerializeType(kafkaMessage.Value, Configuration, out T message) || Convert.TryChangeType(kafkaMessage.Value, out message))
        {
            if (OnAfterSerialization is not null)
                message = OnAfterSerialization.Invoke(message);

            return (true, message);
        }

        return (false, default(T));
    }

    public void SuccessfulConversion(string key, T message)
    {
        if (_logger.IsEnabled(LogLevel.Debug))
            _logger.LogDebug("Message converted successfully to '{TypeName}'", typeof(T).Name);

        OnConsume?.Invoke(new ConsumerMessage<T>(key, message), Consumer.Commit).Wait();
    }

    public void UnsuccessfulConversion(Message<string, string> kafkaMessage)
    {
        var message = $"Unable convert message to {typeof(T).Name}";

        if (_logger.IsEnabled(LogLevel.Debug))
            _logger.LogDebug("{Message}", message);

        throw new KafkaSerializationException(message, kafkaMessage.Value);
    }
}
