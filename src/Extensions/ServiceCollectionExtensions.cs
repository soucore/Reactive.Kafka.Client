using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Reactive.Kafka.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using static Reactive.Kafka.Helpers.Reflection;

namespace Reactive.Kafka.Extensions
{
    public static class ServiceCollectionExtensions
    {
        private static readonly IDictionary<string, int> partitionsDict = new Dictionary<string, int>();
        private static readonly IList<IConsumerWrapper> listConsumerWrapper = new List<IConsumerWrapper>();

        public static IServiceCollection AddReactiveKafkaConsumerPerPartition<T>(this IServiceCollection services, string bootstrapServer, string groupId = default)
            where T : IKafkaConsumer
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

            services.AddSingleton(listConsumerWrapper);
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

        public static IServiceCollection AddReactiveKafkaConsumerPerQuantity<T>(this IServiceCollection services, string bootstrapServer, int quantity, string groupId = default)
            where T : IKafkaConsumer
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

            services.AddSingleton(listConsumerWrapper);
            services.AddTransient(provider =>
            {
                return new ConsumerConfig()
                {
                    BootstrapServers = bootstrapServer,
                    GroupId = groupId
                };
            });

            ApplyConsumerPerQuantity(services.BuildServiceProvider(), typeof(T), quantity);

            return services;
        }

        public static IServiceCollection AddReactiveKafkaConsumer(this IServiceCollection services, string bootstrapServer, string groupId = default)
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

            services.AddSingleton(listConsumerWrapper);
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
        /// Creates consumers from the given assembly.
        /// </summary>
        /// <param name="provider">Dependency injection service provider</param>
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
        /// Creates a consumer per partition of specified topic.
        /// </summary>
        /// <param name="provider">Dependency injection service provider</param>
        /// <param name="consumerType">Consumer type object</param>
        private static void ApplyConsumerPerPartition(IServiceProvider provider, Type consumerType)
        {
            int? partitions = null;
            int consumers = 0;

            do
            {
                IConsumer<string, string> consumer = ApplyReflection(provider, consumerType);

                if (!partitions.HasValue && consumer is not null)
                {
                    string topic = consumer.Subscription.FirstOrDefault();

                    partitions = partitionsDict.ContainsKey(topic)
                        ? partitionsDict[topic]
                        : default(int);
                }
            }
            while (++consumers < partitions);
        }

        /// <summary>
        /// Creates consumers from given quantity.
        /// </summary>
        /// <param name="provider">Dependency injection service provider</param>
        /// <param name="consumerType">Consumer type object</param>
        /// <param name="quantity">Quantity of consumers</param>
        private static void ApplyConsumerPerQuantity(IServiceProvider provider, Type consumerType, int quantity)
        {
            for (int i = 0; i < quantity; i++)
                ApplyReflection(provider, consumerType);
        }

        private static IConsumer<string, string> ApplyReflection(IServiceProvider provider, Type type)
        {
            Type genericTypeArgumentMessage = type
                .GetInterface(typeof(IKafkaConsumer<>).Name, true)?
                .GenericTypeArguments[0];

            if (genericTypeArgumentMessage is null) return default;

            Type consumerWrapperGenericType = typeof(ConsumerWrapper<>)
                .MakeGenericType(genericTypeArgumentMessage);

            var config = provider.GetRequiredService<ConsumerConfig>();

            object consumerInstance = ActivatorUtilities
                .CreateInstance(provider, type);

            type.GetMethod("OnConsumerBuilder")?
                .Invoke(consumerInstance, new object[] { config });

            var builder = new ConsumerBuilder<string, string>(config);
            var consumer = builder.Build();

            type.GetMethod("OnConsumerConfiguration")?
                .Invoke(consumerInstance, new object[] { consumer });

            if (!consumer.Subscription.Any()) return default;

            object consumerWrapperInstance = ActivatorUtilities
                .CreateInstance(provider, consumerWrapperGenericType, new object[] { consumer });

            #region Message Lifecycle
            CreateDelegate(
                consumerWrapperInstance,
                consumerWrapperGenericType.GetEvent("OnBeforeSerialization"),
                consumerInstance,
                type.GetMethod("OnBeforeSerialization"));

            CreateDelegate(
                consumerWrapperInstance,
                consumerWrapperGenericType.GetEvent("OnAfterSerialization"),
                consumerInstance,
                type.GetMethod("OnAfterSerialization"));
            #endregion

            CreateDelegate(
                consumerWrapperInstance,
                consumerWrapperGenericType.GetEvent("OnConsume"),
                consumerInstance,
                type.GetMethod("OnConsume"));

            CreateDelegate(
                consumerWrapperInstance,
                consumerWrapperGenericType.GetEvent("OnConsumeError"),
                consumerInstance,
                type.GetMethod("OnConsumeError"));

            #region Producer Settings
            var producerConfig = new ProducerConfig();

            type.GetMethod("OnProducerBuilder")?
                .Invoke(consumerInstance, new object[] { producerConfig });

            var producerBuilder = new ProducerBuilder<string, string>(producerConfig);
            var producer = producerBuilder.Build();

            object producerWrapperInstance = ActivatorUtilities
                .CreateInstance(provider, typeof(ProducerWrapper), new object[] { producer });

            CreateDelegate(
                consumerInstance,
                type.GetEvent("OnProduce"),
                producerWrapperInstance,
                producerWrapperInstance.GetType().GetMethod("OnProduce"));

            CreateDelegate(
                consumerInstance,
                type.GetEvent("OnProduceAsync"),
                producerWrapperInstance,
                producerWrapperInstance.GetType().GetMethod("OnProduceAsync"));
            #endregion

            consumerWrapperGenericType
                .GetMethod("ConsumerStart")?
                .Invoke(consumerWrapperInstance, Array.Empty<object>());

            listConsumerWrapper.Add((IConsumerWrapper)consumerWrapperInstance);
            return consumer;
        }

        private static void PartitionsDiscovery(string bootstrapServer)
        {
            using var adminClient = new AdminClientBuilder(new AdminClientConfig { BootstrapServers = bootstrapServer }).Build();

            Metadata meta = null;

            meta = adminClient.GetMetadata(TimeSpan.FromSeconds(20));
            meta.Topics.ForEach(topicMetada =>
            {
                if (partitionsDict.ContainsKey(topicMetada.Topic))
                    partitionsDict[topicMetada.Topic] = topicMetada.Partitions.Count;
                else
                    partitionsDict.Add(topicMetada.Topic, topicMetada.Partitions.Count);
            });
        }
        #endregion
    }
}