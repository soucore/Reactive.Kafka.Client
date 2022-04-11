using static System.AppDomain;
using static System.IO.Path;

namespace Reactive.Kafka
{
    public class KafkaHealthCheck
    {
        private readonly IList<IConsumerWrapper> _listConsumerWrapper;
        private readonly ILogger _logger;
        private readonly KafkaHealthCheckConfiguration _config;

        private readonly int referenceTimeMinutes;
        private readonly int second = 1000;

        private readonly string HealthyFile = Combine(CurrentDomain.BaseDirectory, "healthy.txt");
        private readonly string UnHealthyFile = Combine(CurrentDomain.BaseDirectory, "unhealthy.txt");

        public KafkaHealthCheck(IList<IConsumerWrapper> listConsumerWrapper, ILoggerFactory loggerFactory, KafkaHealthCheckConfiguration config)
        {
            _listConsumerWrapper = listConsumerWrapper;
            _config = config;
            _logger = loggerFactory.CreateLogger("Reactive.Kafka.HealthCheck");

            referenceTimeMinutes = config.ReferenceTimeMinutes;
        }

        public DateTime ReferenceDateTime
        {
            get => DateTime.Now.Subtract(
                TimeSpan.FromMinutes(referenceTimeMinutes));
        }

        public Task Start()
        {
            _logger.LogInformation("Initializing health check...");

            return Task.Factory.StartNew(() =>
            {
                bool condition(IConsumerWrapper x) => ReferenceDateTime > x.LastConsume;

                try
                {
                    while (true)
                    {
                        if ((_config.NumberOfObservedConsumers < 0 && _listConsumerWrapper.All(condition)) ||
                            (_config.NumberOfObservedConsumers > 0 && _config.NumberOfObservedConsumers <= _listConsumerWrapper.Count(condition)))
                            UnhealthyStatus();
                        else
                            HealthyStatus();

                        Thread.Sleep(_config.IntervalSeconds * second);
                    }
                }
                catch (IOException ex)
                {
                    LogError(ex);
                }
                catch (Exception ex)
                {
                    LogError(ex);
                    UnhealthyStatus();
                }

            }, TaskCreationOptions.LongRunning);
        }

        public void UnhealthyStatus()
        {
            if (_logger.IsEnabled(LogLevel.Debug))
                _logger.LogDebug("Unhealthy status...");

            if (!File.Exists(UnHealthyFile))
            {
                File.Delete(HealthyFile);
                File.CreateText(UnHealthyFile).Close();
            }
        }

        public void HealthyStatus()
        {
            if (_logger.IsEnabled(LogLevel.Debug))
                _logger.LogDebug("Healthy status...");

            if (!File.Exists(HealthyFile))
            {
                File.Delete(UnHealthyFile);
                File.CreateText(HealthyFile).Close();
            }
        }

        public void LogError(Exception ex)
        {
            if (_logger.IsEnabled(LogLevel.Error))
                _logger.LogError("{ErrorMessage}", ex.ToString());
        }

        public static KafkaHealthCheck CreateInstance(IServiceProvider provider, KafkaHealthCheckConfiguration healthCheckConfig, bool autoStart = true)
        {
            var healthCheck = (KafkaHealthCheck)ActivatorUtilities
                .CreateInstance(provider, typeof(KafkaHealthCheck), new object[] { healthCheckConfig });

            if (autoStart)
                healthCheck.Start();

            return healthCheck;
        }
    }
}
