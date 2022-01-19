using Reactive.Kafka.Validations;

namespace Reactive.Kafka.Interfaces
{
    public interface IKafkaValidation<T>
    {
        void OnValidation(KafkaValidators<T> validators);
    }
}
