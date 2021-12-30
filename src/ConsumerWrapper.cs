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
            _logger = loggerFactory.CreateLogger("Reactive.Kafka.ConsumerWrapper");
            _logger.LogInformation($"Starting consumer {consumer.Name}");

            Consumer = consumer;
            ConsumerStart().Start();
        }

        public IConsumer<string, string> Consumer { get; }

        public Task ConsumerStart()
        {
            _logger.LogInformation($"Initializing consumer {Consumer.Name}");

            return new Task(() =>
            {
                while (true)
                {
                    T message = default;

                    try
                    {
                        var result = Consumer.Consume();

                        _logger.LogDebug($"Message received: {result.Message.Value}");

                        if (Convert.TryChangeType(result.Message.Value, out message) || Convert.TrySerializeType(result.Message.Value, out message))
                        {
                            _logger.LogDebug($"Message converted successfully to {typeof(T).Name}");
                            OnMessage?.Invoke(Consumer, new KafkaEventArgs<T>(result.Message.Key, message));
                        }
                        else
                        {
                            _logger.LogDebug($"Unable convert message to {typeof(T).Name}");
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
                        _logger.LogError($"Unable to consume messages, consumer {Consumer.Name} shutting down.");
                        _logger.LogError(ex.Message);
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
