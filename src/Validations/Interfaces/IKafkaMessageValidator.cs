namespace Reactive.Kafka.Validations.Interfaces
{
    public interface IKafkaMessageValidator<T> : IKafkaMessageValidator
    {
        bool Validate(T message);
    }

    public interface IKafkaMessageValidator { }
}
