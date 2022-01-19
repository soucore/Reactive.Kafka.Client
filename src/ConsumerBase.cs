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
        public virtual void OnConsumerBuilder(ConsumerConfig builder) { }
        public virtual void OnValidation(KafkaValidators<T> validators) { }
        public virtual Task ConsumeError(object sender, KafkaConsumerError consumerError, Commit commit)
        {
            return Task.CompletedTask;
        }

        #region Abstract Methods
        public abstract void OnConsumerConfiguration(IConsumer<string, string> consumer);
        public abstract Task Consume(object sender, KafkaMessage<T> kafkaMessage, Commit commit);
        #endregion
    }
}
