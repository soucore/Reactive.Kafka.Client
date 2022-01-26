using Confluent.Kafka;
using Reactive.Kafka.Errors;
using Reactive.Kafka.Interfaces;
using System.Threading.Tasks;

namespace Reactive.Kafka
{
    public abstract class ConsumerBase<T> : IKafkaConsumer<T>, IKafkaConsumerBuilder, IKafkaConsumerError
    {
        public virtual string OnBeforeSerialization(string rawMessage) => rawMessage;
        public virtual T OnAfterSerialization(T message) => message;

        public virtual void OnConsumerBuilder(ConsumerConfig builder) { }
        public virtual Task OnConsumeError(KafkaConsumerError consumerError, Commit commit)
        {
            return Task.CompletedTask;
        }

        #region Abstract Methods
        public abstract void OnConsumerConfiguration(IConsumer<string, string> consumer);
        public abstract Task OnConsume(ConsumerMessage<T> consumerMessage, Commit commit);
        #endregion
    }
}
