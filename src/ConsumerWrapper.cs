﻿namespace Reactive.Kafka;

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

    private readonly Func<string, KafkaConfiguration, (bool, T, Exception)> convertMessage;
    private readonly string typeName;

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

        typeName = typeof(T).Name;
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

        return Task.Factory
            .StartNew(() => InternalConsume(taskCompletionSource, stoppingToken), stoppingToken, TaskCreationOptions.LongRunning, TaskScheduler.Current)
            .ContinueWith(ContinueWith);

        // We must perfom these actions with a
        // graceful shutdown or a shutdown caused
        // by an unhandled exception.
        void ContinueWith(Task continuationAction)
        {
            ConsumerLogger.LogInformation("Stopping consumer {ConsumerName}", Consumer.Name);
            ConsumerActivity?.Stop();
            Consumer.Close();

            taskCompletionSource?.TrySetResult(null);
        }
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

                MeterHelper.RecordConsumerLag(result);

                ConsumerActivity = ActivityHelper.CreateConsumerActivity(result, Consumer.MemberId, Configuration.ConsumerConfig.GroupId);
                Context.SetResult(result);

                InvokeOnBeforeSerialization(result);

                var (success, message, exception) = convertMessage(result.Message.Value, Configuration);
                if (!success)
                {
                    throw new ConversionException($"Unable convert message to {typeName}", exception);
                }

                InvokeOnAfterSerialization(ref message);

                ConsumerLogger.LogDebug("Message converted successfully to '{TypeName}'", typeName);

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

        using var _ = MeterHelper.RecordConsumerProcessDuration(result);

        // We must use GetWaiter() to not
        // terminate our LongRunning thread.
        OnConsume?.Invoke(consumerMessage, Context, default).GetAwaiter().GetResult();
    }

    private void HandleKafkaOrConsumeException(Exception ex)
    {
        TagList tags = default;

        switch (ex)
        {
            case ConsumeException:
                ConsumerLogger.LogError(ex, "Unable to consume messages from consumer {ConsumerName}.", Consumer.Name);
                break;

            case KafkaException:
                ConsumerLogger.LogError(ex, "{ErrorMessage}", "An internal kafka error has occurred.");
                break;

            case ConversionException:
                ConsumerLogger.LogError(ex, "Unable convert message to {TypeName}", typeof(T).Name);
                tags.Add("exception.kafka.message", Context.ConsumeResult.Message.Value);
                break;
        }

        ConsumerActivity.SetError(ex, tags);

        Context.Exception = ex;
        OnConsumeError?.Invoke(Context).GetAwaiter().GetResult();
    }

    private void HandleUserCodeException(Exception ex)
    {
        ConsumerActivity.SetError(ex);
        ConsumerLogger.LogError(ex, "{ErrorMessage}", "Consumer failed");
    }
    #endregion
}