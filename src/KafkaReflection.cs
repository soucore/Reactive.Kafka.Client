using static Reactive.Kafka.Helpers.Reflection;

namespace Reactive.Kafka
{
    public class KafkaReflection : IKafkaReflection
    {
        private readonly IList<IConsumerWrapper> listConsumerWrapper;
        private readonly IServiceProvider provider;

        public Type type;
        public Type consumerWrapperType;

        public KafkaReflection(
            IServiceProvider provider, Type type, IList<IConsumerWrapper> listConsumerWrapper)
        {
            this.listConsumerWrapper = listConsumerWrapper;
            this.provider = provider;
            this.type = type;

            GenConsumerWrapperType();
        }

        public bool IsTest { get; set; } = false;

        public void GenConsumerWrapperType()
        {
            Type genericTypeArgumentMessage = type
                .GetInterface(typeof(IKafkaConsumer<>).Name, true)
                .GenericTypeArguments[0];

            consumerWrapperType = typeof(ConsumerWrapper<>)
                .MakeGenericType(genericTypeArgumentMessage);
        }

        public (IConsumer<string, string>, IProducer<string, string>) Build(Type alternativeType = null)
        {
            type ??= alternativeType;

            object consumerInstance = ActivatorUtilities
                .CreateInstance(provider, type);

            var consumer = BuildConsumer(consumerInstance);
            var producer = BuildProducer(consumerInstance);

            return (consumer, producer);
        }

        public IConsumer<string, string> BuildConsumer(object consumerInstance)
        {
            var config = provider.GetRequiredService<ConsumerConfig>();

            type.GetMethod("OnConsumerBuilder")?
                .Invoke(consumerInstance, new object[] { config });

            var builder = new ConsumerBuilder<string, string>(config);
            var consumer = builder.Build();

            type.GetMethod("OnConsumerConfiguration")?
                .Invoke(consumerInstance, new object[] { consumer });

            if (!consumer.Subscription.Any())
                return default;

            object consumerWrapperInstance = ActivatorUtilities
                .CreateInstance(provider, consumerWrapperType, new object[] { consumer });

            BindConsumerEvents(consumerInstance, consumerWrapperInstance);

            if (!IsTest)
            {
                consumerWrapperType
                    .GetMethod("ConsumerStart")?
                    .Invoke(consumerWrapperInstance, Array.Empty<object>());
            }

            listConsumerWrapper.Add((IConsumerWrapper)consumerWrapperInstance);
            return consumer;
        }

        public IProducer<string, string> BuildProducer(object consumerInstance)
        {
            var producerConfig = new ProducerConfig();

            type.GetMethod("OnProducerBuilder")?
                .Invoke(consumerInstance, new object[] { producerConfig });

            if (string.IsNullOrEmpty(producerConfig.BootstrapServers))
                return default;

            var producerBuilder = new ProducerBuilder<string, string>(producerConfig);
            var producer = producerBuilder.Build();

            object producerWrapperInstance = ActivatorUtilities
                .CreateInstance(provider, typeof(ProducerWrapper), new object[] { producer });

            BindProducerEvents(consumerInstance, producerWrapperInstance);
            return producer;
        }

        public static void BindConsumerEvents(object consumerInstance, object consumerWrapperInstance)
        {
            CreateDelegate(consumerWrapperInstance, "OnBeforeSerialization", consumerInstance, "OnBeforeSerialization");
            CreateDelegate(consumerWrapperInstance, "OnAfterSerialization", consumerInstance, "OnAfterSerialization");
            CreateDelegate(consumerWrapperInstance, "OnConsume", consumerInstance, "OnConsume");
            CreateDelegate(consumerWrapperInstance, "OnConsumeError", consumerInstance, "OnConsumeError");
        }

        public static void BindProducerEvents(object consumerInstance, object producerWrapperInstance)
        {
            CreateDelegate(consumerInstance, "OnProduce", producerWrapperInstance, "OnProduce");
            CreateDelegate(consumerInstance, "OnProduceAsync", producerWrapperInstance, "OnProduceAsync");
        }

        public static KafkaReflection CreateInstance(IServiceProvider provider, Type type, bool isTest)
        {
            var kafkaReflection = (KafkaReflection)ActivatorUtilities
                .CreateInstance(provider, typeof(KafkaReflection), new object[] { provider, type });

            kafkaReflection.IsTest = isTest;
            return kafkaReflection;
        }
    }
}
