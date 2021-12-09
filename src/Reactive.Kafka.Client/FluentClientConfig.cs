using Confluent.Kafka;

namespace Reactive.Kafka.Client
{
    public class FluentClientConfig
    {
        public ConsumerConfig? ConsumerConfig { get; private set; }
        public ProducerConfig? ProducerConfig { get; private set; }
        public bool IsUseConsumer { get; private set; }
        public bool IsUseProducer { get; private set; }
        public bool IsConsumerMultiThread { get; private set; }

        public FluentClientConfig UseConsumer(ConsumerConfig consumerConfig)
        {
            ConsumerConfig = consumerConfig;
            IsUseConsumer = true;
            return this;
        }

        public FluentClientConfig UseProducer(ProducerConfig producerConfig)
        {
            ProducerConfig = producerConfig;
            IsUseProducer = true;
            return this;
        }

        public FluentClientConfig EnableConsumerMultiThread()
        {
            IsConsumerMultiThread = true;
            return this;
        }
    }
}