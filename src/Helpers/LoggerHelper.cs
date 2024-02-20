#nullable enable
namespace Reactive.Kafka.Helpers;

public class LoggerHelper(ILogger logger)
{
    private readonly ILogger _logger = logger;

    public void LogError(Exception? exception, string? message, params object?[] args)
    {
        if (_logger.IsEnabled(LogLevel.Error))
            _logger.LogError(exception, message, args);
    }

    public void LogInformation(string? message, params object?[] args)
    {
        if (_logger.IsEnabled(LogLevel.Information))
            _logger.LogInformation(message, args);
    }

    public void LogDebug(string? message, params object?[] args)
    {
        if (_logger.IsEnabled(LogLevel.Debug))
            _logger.LogDebug(message, args);
    }
}
