using Reactive.Kafka.Errors;

namespace Reactive.Kafka.Interfaces
{
    public interface IKafkaConsumerError
    {
        void ConsumeError(object sender, KafkaConsumerError consumerError);
    }
}
