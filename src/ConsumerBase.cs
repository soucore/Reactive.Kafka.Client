using Confluent.Kafka;
using Reactive.Kafka.Errors;
using Reactive.Kafka.Interfaces;
using Reactive.Kafka.Validations;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Reactive.Kafka
{
    public abstract class ConsumerBase<T> : IKafkaConsumer<T>, IKafkaConsumerBuilder, IKafkaConsumerError, IKafkaValidation<T>
    {
        #region Message Lifecycle
        public virtual string OnBeforeSerialization(string rawMessage) => rawMessage;
        public virtual T OnAfterSerialization(T message) => message;
        #endregion

        public virtual void OnConsumerBuilder(ConsumerConfig builder) { }
        public virtual void OnValidation(KafkaValidators<T> validators) { }
        public virtual Task ConsumeError(KafkaConsumerError consumerError, Commit commit)
        {
            return Task.CompletedTask;
        }

        #region Abstract Methods
        public abstract void OnConsumerConfiguration(IConsumer<string, string> consumer);
        public abstract Task Consume(ConsumerMessage<T> consumerMessage, Commit commit);
        #endregion
    }
}
