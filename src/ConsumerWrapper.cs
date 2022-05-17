using Convert = Reactive.Kafka.Helpers.Convert;

namespace Reactive.Kafka
{
    #region Custom Delegates
    public delegate List<TopicPartitionOffset> Commit();
    public delegate Task EventHandlerAsync<in TMessage>(TMessage e, Commit commit);
    #endregion

    public sealed class ConsumerWrapper<T> : IConsumerWrapper
    {
        #region Events
        public event Func<string, string> OnBeforeSerialization;
        public event Func<T, T> OnAfterSerialization;
        public event EventHandlerAsync<ConsumerMessage<T>> OnConsume;
        public event EventHandlerAsync<KafkaConsumerError> OnConsumeError;
        #endregion

        private readonly ILogger _logger;

        public ConsumerWrapper(ILoggerFactory loggerFactory, IConsumer<string, string> consumer)
        {
            _logger = loggerFactory.CreateLogger("Reactive.Kafka");

            if (_logger.IsEnabled(LogLevel.Information))
                _logger.LogInformation("Creating consumer {ConsumerName}", consumer.Name);

            Consumer = consumer;
        }

        public IConsumer<string, string> Consumer { get; }
        public DateTime LastConsume { get; private set; } = DateTime.Now;

        public Task ConsumerStart()
        {
            if (_logger.IsEnabled(LogLevel.Information))
                _logger.LogInformation("Initializing consumer {ConsumerName}", Consumer.Name);

            return Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    try
                    {
                        var message = ConsumerMessage();
                        ConvertMessage(message);
                    }
                    catch (KafkaConsumerException ex)
                    {
                        if (OnConsumeError is null)
                            continue;

                        _ = HandleException(ex);
                    }
                    catch (ConsumeException ex)
                    {
                        if (_logger.IsEnabled(LogLevel.Error))
                        {
                            _logger.LogError("Unable to consume messages, consumer {ConsumerName} shutting down.", Consumer.Name);
                            _logger.LogError("{Message}", ex.Message);
                        }
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
            }, TaskCreationOptions.LongRunning);
        }

        public Message<string, string> ConsumerMessage()
        {
            var result = Consumer.Consume();

            if (_logger.IsEnabled(LogLevel.Debug))
                _logger.LogDebug("Message received: {MessageValue}", result.Message.Value);

            LastConsume = DateTime.Now;

            return result.Message;
        }

        public void ConvertMessage(Message<string, string> kafkaMessage)
        {
            if (OnBeforeSerialization is not null)
                kafkaMessage.Value = OnBeforeSerialization.Invoke(kafkaMessage.Value);

            T message = default;
            if (Convert.TryChangeType(kafkaMessage.Value, out message) || Convert.TrySerializeType(kafkaMessage.Value, out message))
            {
                if (OnAfterSerialization is not null)
                    message = OnAfterSerialization.Invoke(message);

                _ = SuccessfulConversion(message, kafkaMessage);
            }
            else
            {
                UnsuccessfulConversion(kafkaMessage);
            }
        }

        public async Task HandleException(KafkaConsumerException exception)
        {
            await OnConsumeError.Invoke(new KafkaConsumerError(exception), Consumer.Commit);
        }

        public async Task SuccessfulConversion(T message, Message<string, string> kafkaMessage)
        {
            if (_logger.IsEnabled(LogLevel.Debug))
                _logger.LogDebug("Message converted successfully to '{TypeName}'", typeof(T).Name);

            if (OnConsume is not null)
                await OnConsume.Invoke(new ConsumerMessage<T>(kafkaMessage.Key, message), Consumer.Commit)!;
        }

        public void UnsuccessfulConversion(Message<string, string> kafkaMessage)
        {
            var message = $"Unable convert message to {typeof(T).Name}";
            if (_logger.IsEnabled(LogLevel.Debug))
                _logger.LogDebug(message);

            throw new KafkaSerializationException(message, kafkaMessage.Value);
        }
    }
}
