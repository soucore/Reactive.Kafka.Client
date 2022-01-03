using Confluent.Kafka;
using Reactive.Kafka.Errors;
using Reactive.Kafka.Interfaces;

namespace Reactive.Kafka
{
    public abstract class ConsumerBase<T> : IKafkaConsumer<T>
    {
        public virtual void OnConsumerBuilder(ConsumerConfig builder) { }
        public virtual void ConsumeError(object sender, KafkaConsumerError consumerError) { }

        #region Abstract Methods
        public abstract void OnConsumerConfiguration(IConsumer<string, string> consumer);
        public abstract void Consume(object sender, KafkaEventArgs<T> @event);
        #endregion
    }
}
