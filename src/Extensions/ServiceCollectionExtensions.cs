using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Reactive.Kafka.Interfaces;
using System.Reflection;

namespace Reactive.Kafka.Extensions
{
    public static class ServiceCollectionExtensions
    {
        private static readonly IDictionary<string, int> partitionsDict = new Dictionary<string, int>();

        public static IServiceCollection AddReactiveKafkaConsumerPerPartition<T>(this IServiceCollection services, string bootstrapServer, string groupId = default)
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (bootstrapServer is null)
            {
                throw new ArgumentNullException(nameof(bootstrapServer));
            }

            groupId ??= Guid.NewGuid().ToString();

            services.AddTransient(provider =>
            {
                return new ConsumerConfig()
                {
                    BootstrapServers = bootstrapServer,
                    GroupId = groupId
                };
            });

            PartitionsDiscovery(bootstrapServer);
            ApplyConsumerPerPartition(services.BuildServiceProvider(), typeof(T));

            return services;
        }

        public static IServiceCollection AddReactiveKafkaConsumer(this IServiceCollection services, string bootstrapServer, string groupId = default)
        {
            if (bootstrapServer is null)
            {
                throw new ArgumentNullException(nameof(bootstrapServer));
            }

            if (groupId is null)
            {
                groupId = Guid.NewGuid().ToString();
            }

            return services.AddReactiveKafkaConsumer(config =>
            {
                config.BootstrapServers = bootstrapServer;
                config.GroupId = groupId;
            }, Assembly.GetCallingAssembly());
        }

        public static IServiceCollection AddReactiveKafkaConsumer(this IServiceCollection services, Action<ConsumerConfig> setupAction, Assembly assembly = default)
        {
            if (setupAction is null)
            {
                throw new ArgumentNullException(nameof(setupAction));
            }

            services.AddTransient(provider =>
            {
                ConsumerConfig config = new();
                setupAction(config);

                return config;
            });

            assembly ??= Assembly.GetCallingAssembly();

            ApplyConsumersFromAssembly(
                services.BuildServiceProvider(), assembly);

            return services;
        }

        #region Non-Public Methods        
        /// <summary>
        /// Create consumers from supplied assembly.
        /// </summary>
        /// <param name="provider">Dependency injection provider</param>
        /// <param name="assembly">Assembly from which search will be done</param>
        private static void ApplyConsumersFromAssembly(IServiceProvider provider, Assembly assembly)
        {
            IEnumerable<Type> types = assembly
                .GetTypes()
                .Where(type => type.GetInterface(typeof(IKafkaConsumer<>).Name, true) is not null);

            foreach (Type type in types)
                ApplyReflection(provider, type);
        }

        /// <summary>
        /// Create consumer per partition of specified topic.
        /// </summary>
        /// <param name="provider">Dependency injection provider</param>
        /// <param name="consumerType">Consumer type object</param>
        private static void ApplyConsumerPerPartition(IServiceProvider provider, Type consumerType)
        {
            int? partitions = null;
            int consumers = 0;

            do
            {
                IConsumer<string, string> consumer = ApplyReflection(provider, consumerType);

                if (!partitions.HasValue)
                {
                    string topic = consumer.Subscription.FirstOrDefault();

                    partitions = partitionsDict.ContainsKey(topic)
                        ? partitionsDict[topic]
                        : default(int);
                }
            }
            while (++consumers < partitions);
        }

        private static IConsumer<string, string> ApplyReflection(IServiceProvider provider, Type type)
        {
            var config = provider.GetRequiredService<ConsumerConfig>();

            object consumerInstance = ActivatorUtilities
                .CreateInstance(provider, type);

            Type genericTypeArgumentMessage = type
                .GetInterface(typeof(IKafkaConsumer<>).Name, true)?
                .GenericTypeArguments[0];

            Type consumerWrapperGenericType = typeof(ConsumerWrapper<>)
                .MakeGenericType(genericTypeArgumentMessage);

            type.GetMethod("OnConsumerBuilder")?
                .Invoke(consumerInstance, new object[] { config });

            var builder = new ConsumerBuilder<string, string>(config);
            var consumer = builder.Build();

            type.GetMethod("OnConsumerConfiguration")?
                .Invoke(consumerInstance, new object[] { consumer });

            object consumerWrapperInstance = ActivatorUtilities
                .CreateInstance(provider, consumerWrapperGenericType, new object[] { consumer });

            EventInfo eventInfoOnMessage = consumerWrapperGenericType.GetEvent("OnMessage");
            EventInfo eventInfoOnError = consumerWrapperGenericType.GetEvent("OnError");

            MethodInfo consumeMethod = type.GetMethod("Consume");
            if (consumeMethod is not null)
            {
                Delegate consumeDelegate = Delegate
                    .CreateDelegate(
                        eventInfoOnMessage.EventHandlerType,
                        consumerInstance,
                        consumeMethod);

                eventInfoOnMessage.AddEventHandler(consumerWrapperInstance, consumeDelegate);
            }

            MethodInfo consumeErrorMethod = type.GetMethod("ConsumeError");
            if (consumeErrorMethod is not null)
            {
                Delegate consumeErrorDelegate = Delegate
                    .CreateDelegate(
                        eventInfoOnError.EventHandlerType,
                        consumerInstance,
                        consumeErrorMethod);

                eventInfoOnError.AddEventHandler(consumerWrapperInstance, consumeErrorDelegate);
            }

            return consumer;
        }

        private static void PartitionsDiscovery(string bootstrapServer)
        {
            using var adminClient = new AdminClientBuilder(new AdminClientConfig { BootstrapServers = bootstrapServer }).Build();

            Metadata meta = null;

            meta = adminClient.GetMetadata(TimeSpan.FromSeconds(20));
            meta.Topics.ForEach(topicMetada => partitionsDict.Add(topicMetada.Topic, topicMetada.Partitions.Count));
        }
        #endregion
    }
}