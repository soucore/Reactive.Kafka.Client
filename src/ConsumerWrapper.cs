using Reactive.Kafka.Helpers;
using System.Diagnostics;

namespace Reactive.Kafka;

#region Custom Delegates
public delegate Task EventHandlerAsync<in TMessage>(TMessage e, ConsumerContext context, CancellationToken cancellationToken);
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

    public ConsumerWrapper(ILoggerFactory loggerFactory, IConsumer<string, string> consumer, KafkaConfiguration configuration)
    {
        ConsumerLogger = new(loggerFactory.CreateLogger("Reactive.Kafka.Consumer"));
        ConsumerLogger.LogInformation("Creating consumer {ConsumerName}", consumer.Name);

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
    public Activity ConsumerActivity { get; private set; }
    public LoggerHelper ConsumerLogger { get; }
    #endregion

    public Task ConsumerStart(TaskCompletionSource<object> taskCompletionSource, CancellationToken stoppingToken)
    {
        ConsumerLogger.LogInformation("Initializing consumer {ConsumerName}", Consumer.Name);

        return Task.Factory.StartNew(
            () => InternalConsume(taskCompletionSource, stoppingToken), stoppingToken, TaskCreationOptions.LongRunning, TaskScheduler.Current);
    }

    #region Non-Public Methods
    private void InternalConsume(TaskCompletionSource<object> taskCompletionSource, CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                ConsumerActivity?.Stop();

                var result = Consumer.Consume(stoppingToken);
                if (result is null)
                    continue;

                ConsumerActivity = ActivityHelper.CreateConsumerActivity(result, Consumer.MemberId);
                Context.SetResult(result);

                InvokeOnBeforeSerialization(result);

                var (success, message) = convertMessage(result.Message.Value, Configuration);
                if (!success)
                {
                    throw new ConversionException($"Unable convert message to {typeof(T).Name}");
                }

                InvokeOnAfterSerialization(ref message);

                ConsumerLogger.LogDebug("Message converted successfully to '{TypeName}'", typeof(T).Name);

                InvokeOnConsume(result, message);
            }
            catch (Exception ex) when (ex is ConsumeException or KafkaException or ConversionException)
            {
                HandleKafkaOrConsumeException(ex);
            }
            catch (OperationCanceledException)
            {
                continue;
            }
            catch (Exception ex)
            {
                HandleUserCodeException(ex);
                throw;
            }
        }

        ConsumerLogger.LogInformation("Stopping consumer {ConsumerName}", Consumer.Name);
        ConsumerActivity?.Stop();
        Consumer.Close();

        taskCompletionSource?.TrySetResult(null);
    }

    private void InvokeOnBeforeSerialization(ConsumeResult<string, string> result)
    {
        if (OnBeforeSerialization is not null)
            result.Message.Value = OnBeforeSerialization.Invoke(result.Message.Value);
    }

    private void InvokeOnAfterSerialization(ref T message)
    {
        if (OnAfterSerialization is not null)
            message = OnAfterSerialization.Invoke(message);
    }

    private void InvokeOnConsume(ConsumeResult<string, string> result, T message)
    {
        var consumerMessage = new ConsumerMessage<T>(result.Message.Key, message);
        OnConsume?.Invoke(consumerMessage, Context, default).Wait(CancellationToken.None);
    }

    private void HandleKafkaOrConsumeException(Exception ex)
    {
        ConsumerActivity.SetError(ex);

        if (ex is ConsumeException)
            ConsumerLogger.LogError(ex, "Unable to consume messages from consumer {ConsumerName}.", Consumer.Name);

        if (ex is KafkaException)
            ConsumerLogger.LogError(ex, "{ErrorMessage}", "An internal kafka error has occurred.");

        if (ex is KafkaSerializationException)
            ConsumerLogger.LogError(ex, "Unable convert message to {TypeName}", typeof(T).Name);

        Context.Exception = ex;
        OnConsumeError?.Invoke(Context).Wait();
    }

    private void HandleUserCodeException(Exception ex)
    {
        ConsumerActivity.SetError(ex);
        ConsumerLogger.LogError(ex, "{ErrorMessage}", "Consumer failed");
    }
    #endregion
}