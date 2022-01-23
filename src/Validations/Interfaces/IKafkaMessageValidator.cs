using System.Threading.Tasks;

namespace Reactive.Kafka.Validations.Interfaces
{
    public interface IKafkaMessageValidator<T> : IKafkaMessageValidator
    {
        ValueTask<bool> Validate(T message);
    }

    public interface IKafkaMessageValidator { }
}
