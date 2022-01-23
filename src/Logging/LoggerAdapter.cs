using Microsoft.Extensions.Logging;
using System;

namespace Reactive.Kafka.Logging
{
    internal sealed class LoggerAdapter : ILogger
    {
        private readonly ILogger _logger;

        public LoggerAdapter(ILogger logger)
            => _logger = logger;

        public IDisposable BeginScope<TState>(TState state)
        {
            return _logger.BeginScope(state);
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return _logger.IsEnabled(logLevel);
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (_logger.IsEnabled(logLevel))
                _logger.Log(logLevel, eventId, state, exception, formatter);
        }
    }
}
