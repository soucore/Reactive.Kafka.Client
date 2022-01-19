using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Reactive.Kafka.Errors;
using Reactive.Kafka.Exceptions;
using Reactive.Kafka.Interfaces;
using Reactive.Kafka.Validations;
using Reactive.Kafka.Validations.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Convert = Reactive.Kafka.Helpers.Convert;

namespace Reactive.Kafka
{
    #region Custom Delegates
    public delegate List<TopicPartitionOffset> Commit();
    public delegate Task EventHandlerAsync<TMessage>(object sender, TMessage e, Commit commit);
    #endregion

    internal sealed class ConsumerWrapper<T> : IConsumerWrapper
    {
        #region Events
        public event EventHandlerAsync<KafkaMessage<T>> OnMessage;
        public event EventHandlerAsync<KafkaConsumerError> OnError;
        #endregion

        private readonly KafkaValidators<T> _validators;
        private readonly ILogger _logger;

        public ConsumerWrapper(ILoggerFactory loggerFactory, IConsumer<string, string> consumer, KafkaValidators<T> validators)
        {
            _validators = validators.Count > 0
                ? validators : null;

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

            return new Task(async () =>
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
                            await SuccessfulConversion(message, result);
                        }
                        else
                        {
                            await UnsuccessfulConversion(result);
                        }
                    }
                    catch (Exception ex) when (ex is KafkaConsumerException || ex is KafkaValidationException)
                    {
                        if (OnError is null)
                            continue;

                        await OnError.Invoke(this, new(ex), Consumer.Commit);
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

        #region Non-Public Methods
        private async Task SuccessfulConversion(T message, ConsumeResult<string, string> result)
        {
            if (_logger.IsEnabled(LogLevel.Debug))
                _logger.LogDebug("Message converted successfully to {TypeName}", typeof(T).Name);

            if (_validators is not null)
                MessageValidation(message, result);

            await OnMessage?.Invoke(Consumer, new KafkaMessage<T>(result.Message.Key, message), Consumer.Commit);
        }

        private Task UnsuccessfulConversion(ConsumeResult<string, string> result)
        {
            if (_logger.IsEnabled(LogLevel.Debug))
                _logger.LogDebug("Unable convert message to {TypeName}", typeof(T).Name);

            throw new KafkaConsumerException(result.Message.Value);
        }

        private void MessageValidation(T message, ConsumeResult<string, string> result)
        {
            foreach (IKafkaMessageValidator<T> validator in _validators)
            {
                if (!validator.Validate(message))
                    throw new KafkaValidationException(result.Message.Value);
            }
        }
        #endregion
    }
}
