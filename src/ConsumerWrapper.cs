using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Reactive.Kafka.Errors;
using Reactive.Kafka.Exceptions;
using Convert = Reactive.Kafka.Helpers.Convert;

namespace Reactive.Kafka
{
    internal sealed class ConsumerWrapper<T>
    {
        #region Events
        public event EventHandler<KafkaEventArgs<T>> OnMessage;
        public event EventHandler<KafkaConsumerError> OnError;
        #endregion

        private readonly ILogger _logger;

        public ConsumerWrapper(ILoggerFactory loggerFactory, IConsumer<string, string> consumer)
        {
            _logger = loggerFactory.CreateLogger("Reactive.Kafka");

            if (_logger.IsEnabled(LogLevel.Information))
                _logger.LogInformation("Starting consumer {ConsumerName}", consumer.Name);

            Consumer = consumer;
            ConsumerStart().Start();
        }

        public IConsumer<string, string> Consumer { get; }

        public Task ConsumerStart()
        {
            if (_logger.IsEnabled(LogLevel.Information))
                _logger.LogInformation("Initializing consumer {ConsumerName}", Consumer.Name);

            return new Task(() =>
            {
                while (true)
                {
                    T message = default;

                    try
                    {
                        var result = Consumer.Consume();

                        if (_logger.IsEnabled(LogLevel.Debug))
                            _logger.LogDebug("Message received: {MessageValue}", result.Message.Value);

                        if (Convert.TryChangeType(result.Message.Value, out message) || Convert.TrySerializeType(result.Message.Value, out message))
                        {
                            if (_logger.IsEnabled(LogLevel.Debug))
                                _logger.LogDebug("Message converted successfully to {TypeName}", typeof(T).Name);

                            OnMessage?.Invoke(Consumer, new KafkaEventArgs<T>(result.Message.Key, message));
                        }
                        else
                        {
                            if (_logger.IsEnabled(LogLevel.Debug))
                                _logger.LogDebug("Unable convert message to {TypeName}", typeof(T).Name);

                            throw new KafkaConsumerException(result.Message.Value);
                        }
                    }
                    catch (KafkaConsumerException ex)
                    {
                        KafkaConsumerError kafkaConsumerError = new()
                        {
                            Exception = ex,
                            KafkaMessage = ex.KafkaMessage
                        };

                        OnError?.Invoke(this, kafkaConsumerError);
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
            });
        }
    }
}
