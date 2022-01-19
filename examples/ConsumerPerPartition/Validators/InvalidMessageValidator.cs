using Reactive.Kafka.Validations.Interfaces;

namespace ConsumerPerPartition.Validators
{
    internal class InvalidMessageValidator : IKafkaMessageValidator<Message>
    {
        public bool Validate(Message message) => message.Id > 0;
    }
}
