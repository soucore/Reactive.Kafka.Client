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

        public ConsumerWrapper(ILoggerFactory loggerFactory, IConsumer<string, string> consumer, KafkaConfiguration configuration)
        {
            _logger = loggerFactory.CreateLogger("Reactive.Kafka");

            if (_logger.IsEnabled(LogLevel.Information))
                _logger.LogInformation("Creating consumer {ConsumerName}", consumer.Name);

            Consumer = consumer;
            Configuration = configuration;
        }

        public IConsumer<string, string> Consumer { get; }
        public KafkaConfiguration Configuration { get; }
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
                        Message<string, string> consumedMessage = ConsumeMessage();

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
                        if (OnConsumeError is null)
                            continue;

                        var task = OnConsumeError.Invoke(new KafkaConsumerError(ex), Consumer.Commit);

                        if (Configuration.WaitNextConsume)
                            task.Wait();
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

        public Message<string, string> ConsumeMessage()
        {
            var result = Consumer.Consume();

            if (_logger.IsEnabled(LogLevel.Debug))
                _logger.LogDebug("Message received: {MessageValue}", result.Message.Value);

            LastConsume = DateTime.Now;

            return result.Message;
        }

        public (bool, T) ConvertMessage(Message<string, string> kafkaMessage)
        {
            if (OnBeforeSerialization is not null)
                kafkaMessage.Value = OnBeforeSerialization.Invoke(kafkaMessage.Value);

            if (Convert.TryChangeType(kafkaMessage.Value, out T message) || Convert.TrySerializeType(kafkaMessage.Value, Configuration.RespectObjectContract, out message))
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

            if (OnConsume is not null)
            {
                var task = OnConsume.Invoke(new ConsumerMessage<T>(key, message), Consumer.Commit);

                if (Configuration.WaitNextConsume)
                    task.Wait();
            }
        }

        public void UnsuccessfulConversion(Message<string, string> kafkaMessage)
        {
            var message = $"Unable convert message to {typeof(T).Name}";

            if (_logger.IsEnabled(LogLevel.Debug))
                _logger.LogDebug("{Message}", message);

            throw new KafkaSerializationException(message, kafkaMessage.Value);
        }
    }
}
