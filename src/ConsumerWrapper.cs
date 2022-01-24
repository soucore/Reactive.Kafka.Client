using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Reactive.Kafka.Errors;
using Reactive.Kafka.Exceptions;
using Reactive.Kafka.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        public event EventHandlerAsync<ConsumerMessage<T>> OnMessage;
        public event EventHandlerAsync<KafkaConsumerError> OnError;
        #endregion

        private readonly ILogger _logger;

        public ConsumerWrapper(ILoggerFactory loggerFactory, IConsumer<string, string> consumer)
        {
            _logger = loggerFactory.CreateLogger("Reactive.Kafka");

            if (_logger.IsEnabled(LogLevel.Information))
                _logger.LogInformation("Creating consumer {ConsumerName}", consumer.Name);

            Consumer = consumer;
            ConsumerStart();
        }

        public IConsumer<string, string> Consumer { get; }

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
                        var result = Consumer.Consume();

                        if (_logger.IsEnabled(LogLevel.Debug))
                            _logger.LogDebug("Message received: {MessageValue}", result.Message.Value);

                        _ = ConvertMessage(result.Message);
                    }
                    catch (KafkaConsumerException ex)
                    {
                        if (OnError is not null)
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

        public async Task ConvertMessage(Message<string, string> kafkaMessage)
        {
            if (OnBeforeSerialization is not null)
                kafkaMessage.Value = OnBeforeSerialization.Invoke(kafkaMessage.Value);

            T message = default;
            if (Convert.TryChangeType(kafkaMessage.Value, out message) || Convert.TrySerializeType(kafkaMessage.Value, out message))
            {
                if (OnAfterSerialization is not null)
                    message = OnAfterSerialization.Invoke(message);

                await SuccessfulConversion(message, kafkaMessage);
            }
            else
            {
                UnsuccessfulConversion(kafkaMessage);
            }
        }

        public async Task HandleException(Exception exception)
        {
            await OnError.Invoke(new KafkaConsumerError(exception), Consumer.Commit);
        }

        #region Non-Public Methods
        private async Task SuccessfulConversion(T message, Message<string, string> kafkaMessage)
        {
            if (_logger.IsEnabled(LogLevel.Debug))
                _logger.LogDebug("Message converted successfully to '{TypeName}'", typeof(T).Name);

            if (OnMessage is not null)
                await OnMessage.Invoke(new ConsumerMessage<T>(kafkaMessage.Key, message), Consumer.Commit)!;
        }

        private void UnsuccessfulConversion(Message<string, string> kafkaMessage)
        {
            if (_logger.IsEnabled(LogLevel.Debug))
                _logger.LogDebug("Unable convert message to '{TypeName}'", typeof(T).Name);

            throw new KafkaConsumerException(kafkaMessage.Value);
        }
        #endregion
    }
}
