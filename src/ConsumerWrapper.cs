using Reactive.Kafka.Helpers;

namespace Reactive.Kafka;

#region Custom Delegates
public delegate Task EventHandlerAsync<in TMessage>(TMessage e, ConsumerContext context);
#endregion

public sealed class ConsumerWrapper<T> : IConsumerWrapper<T>
{
    #region Events
    public event Func<string, string> OnBeforeSerialization;
    public event Func<T, T> OnAfterSerialization;
    public event Func<ConsumerContext, Task> OnConsumeError;
    public event EventHandlerAsync<ConsumerMessage<T>> OnConsume;
    #endregion

    private readonly Func<string, KafkaConfiguration, (bool, T)> convertMessage;
    private readonly ILogger _logger;

    public ConsumerWrapper(ILoggerFactory loggerFactory, IConsumer<string, string> consumer, KafkaConfiguration configuration)
    {
        _logger = loggerFactory.CreateLogger("Reactive.Kafka.Consumer");

        if (_logger.IsEnabled(LogLevel.Information))
            _logger.LogInformation("Creating consumer {ConsumerName}", consumer.Name);

        Consumer = consumer;
        Configuration = configuration;
        Context = new() { Consumer = consumer };

        convertMessage = Type.GetTypeCode(typeof(T)) == TypeCode.Object
            ? Convert<T>.TrySerializeType
            : Convert<T>.TryChangeType;
    }

    #region Properties
    public IConsumer<string, string> Consumer { get; }
    public KafkaConfiguration Configuration { get; }
    public DateTime LastConsume { get; private set; } = DateTime.Now;
    public ConsumerContext Context { get; }
    #endregion

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
                    var consumeResult = ConsumeMessage(stoppingToken);
                    if (consumeResult is null)
                        continue;

                    Context.SetResult(consumeResult);

                    if (OnBeforeSerialization is not null)
                        consumeResult.Message.Value = OnBeforeSerialization.Invoke(consumeResult.Message.Value);

                    (bool successfulConversion, T message) = convertMessage(consumeResult.Message.Value, Configuration);

                    if (successfulConversion)
                    {
                        if (OnAfterSerialization is not null)
                            message = OnAfterSerialization.Invoke(message);

                        var consumerMessage = new ConsumerMessage<T>(consumeResult.Message.Key, message);

                        SuccessfulConversion(consumerMessage, Context);
                        continue;
                    }

                    UnsuccessfulConversion(consumeResult.Message);
                }
                catch (KafkaConsumerException ex)
                {
                    Context.Exception = ex;
                    OnConsumeError?.Invoke(Context).Wait();
                }
                catch (ConsumeException ex)
                {
                    if (_logger.IsEnabled(LogLevel.Error))
                        _logger.LogError(ex, "Unable to consume messages from consumer {ConsumerName}.", Consumer.Name);

                    Context.Exception = ex;
                    OnConsumeError?.Invoke(Context).Wait();
                }
                catch (KafkaException ex)
                {
                    if (_logger.IsEnabled(LogLevel.Error))
                        _logger.LogError(ex, "An internal kafka error has occurred.");

                    Context.Exception = ex;
                    OnConsumeError?.Invoke(Context).Wait();
                }
                catch (Exception ex)
                {
                    if (_logger.IsEnabled(LogLevel.Error))
                        _logger.LogError(ex, "Consumer failed", ex.Message);

                    throw;
                }
            }

            _logger.LogInformation("Stopping consumer {ConsumerName}", Consumer.Name);

            Consumer.Close();

            taskCompletionSource?.TrySetResult(null);

        }, stoppingToken, TaskCreationOptions.LongRunning, TaskScheduler.Current);
    }

    public ConsumeResult<string, string> ConsumeMessage(CancellationToken stoppingToken)
    {
        try
        {
            var result = Consumer.Consume(stoppingToken);

            if (result != null && _logger.IsEnabled(LogLevel.Debug))
                _logger.LogDebug("Message received from partition {Partition}: {MessageValue}", result.Partition.Value, result.Message.Value);

            return result;
        }
        catch (OperationCanceledException)
        {
            return null;
        }
    }

    public void SuccessfulConversion(ConsumerMessage<T> consumerMessage, ConsumerContext context)
    {
        if (_logger.IsEnabled(LogLevel.Debug))
            _logger.LogDebug("Message converted successfully to '{TypeName}'", typeof(T).Name);

        OnConsume?.Invoke(consumerMessage, context).Wait();
    }

    public void UnsuccessfulConversion(Message<string, string> kafkaMessage)
    {
        var message = $"Unable convert message to {typeof(T).Name}";

        if (_logger.IsEnabled(LogLevel.Debug))
            _logger.LogDebug("{Message}", message);

        throw new KafkaSerializationException(message, kafkaMessage.Value);
    }
}
