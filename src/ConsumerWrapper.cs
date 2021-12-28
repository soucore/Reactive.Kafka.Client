using Confluent.Kafka;
using Reactive.Kafka.Errors;
using Reactive.Kafka.Exceptions;
using Convert = Reactive.Kafka.Helpers.Convert;

namespace Reactive.Kafka
{
    internal sealed class ConsumerWrapper<T>
    {
        public event EventHandler<KafkaEventArgs<T>> OnMessage;
        public event EventHandler<KafkaConsumerError> OnError;
        private readonly IConsumer<string, string> Consumer;

        public ConsumerWrapper(IConsumer<string, string> consumer)
        {
            Consumer = consumer;
            ConsumerStart().Start();
        }

        public Task ConsumerStart()
        {
            return new Task(() =>
            {
                while (true)
                {
                    T message = default;

                    try
                    {
                        var result = Consumer.Consume();

                        if (Convert.TryChangeType(result.Message.Value, out message) || Convert.TrySerializeType(result.Message.Value, out message))
                            OnMessage?.Invoke(Consumer, new KafkaEventArgs<T>(result.Message.Key, message));
                        else
                            throw new KafkaConsumerException(result.Message.Value);
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
                    catch (Exception)
                    {
                        throw;
                    }
                }
            });
        }
    }
}
