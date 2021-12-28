using Confluent.Kafka;

namespace WorkerService1
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = "localhost:9092",
                GroupId = "foo",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };


             await ConsumerStart(config);
        }

        public Task ConsumerStart(ConsumerConfig config)
        {
            using var Consumer = new ConsumerBuilder<Ignore, string>(config).Build();
            Consumer.Subscribe("teste-topic");
            ConsumeResult<Ignore, string>? result = null;

            while (true)
            {
                try
                {
                    result = Consumer.Consume();
                    Console.WriteLine($"[Key] => {result.Message.Key}");
                    Console.WriteLine($"[Message] => {result.Message.Value}");
                    Console.WriteLine($"_____________________________________________________________");
                }

                catch (Exception)
                {
                    throw;
                }
            }
        }
    }

    public class Message
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }
}