namespace Reactive.Kafka.Interfaces
{
    public interface IKafkaReflection
    {
        (IConsumer<string, string>, IProducer<string, string>) Build();
        IConsumer<string, string> BuildConsumer(object consumerInstance);
        IProducer<string, string> BuildProducer(object consumerInstance);
    }
}
