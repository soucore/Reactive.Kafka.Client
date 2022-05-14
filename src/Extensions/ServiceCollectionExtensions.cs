﻿namespace Reactive.Kafka.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static readonly IList<IConsumerWrapper> listConsumerWrapper = new List<IConsumerWrapper>();

        public static IServiceCollection AddReactiveKafkaConsumerPerPartition<T>(this IServiceCollection services, string bootstrapServer, string groupId = default)
            where T : IKafkaConsumer
        {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(bootstrapServer);

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

            ApplyConsumerPerPartition(services.BuildServiceProvider(), typeof(T), bootstrapServer);
            return services;
        }

        public static IServiceCollection AddReactiveKafkaConsumerPerQuantity<T>(this IServiceCollection services, string bootstrapServer, int quantity, string groupId = default)
            where T : IKafkaConsumer
        {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(bootstrapServer);

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
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(bootstrapServer);

            groupId ??= Guid.NewGuid().ToString();

            return services.AddReactiveKafkaConsumer(config =>
            {
                config.BootstrapServers = bootstrapServer;
                config.GroupId = groupId;
            }, Assembly.GetCallingAssembly());
        }

        public static IServiceCollection AddReactiveKafkaConsumer(this IServiceCollection services, Action<ConsumerConfig> setupAction, Assembly assembly = default)
        {
            ArgumentNullException.ThrowIfNull(setupAction);

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

        public static IServiceCollection AddReactiveKafkaHealthCheck(this IServiceCollection services, Action<KafkaHealthCheckConfiguration> setupAction = null)
        {
            IServiceProvider provider = services.BuildServiceProvider();

            var config = (KafkaHealthCheckConfiguration)ActivatorUtilities
                .CreateInstance(provider, typeof(KafkaHealthCheckConfiguration));

            setupAction?.Invoke(config);

            KafkaHealthCheck
                .CreateInstance(provider, config, autoStart: true);

            return services;
        }

        public static void ApplyConsumersFromAssembly(IServiceProvider provider, Assembly assembly, bool test = false)
        {
            IEnumerable<Type> types = assembly
                .GetTypes()
                .Where(type => type.GetInterface(typeof(IKafkaConsumer<>).Name, true) is not null);

            foreach (Type type in types)
            {
                var kafkaReflection = KafkaReflection.CreateInstance(provider, type, test);
                kafkaReflection.Build();
            }
        }

        public static void ApplyConsumerPerPartition(IServiceProvider provider, Type consumerType, string bootstrapServer, bool test = false)
        {
            int? partitions = null;
            int? consumers = 0;

            var kafkaAdmin = KafkaAdmin.CreateInstance(provider, bootstrapServer, test);
            var kafkaReflection = KafkaReflection.CreateInstance(provider, consumerType, test);

            Dictionary<string, int> partitionsDict = kafkaAdmin.PartitionsDiscovery();

            do
            {
                var (consumer, _) = kafkaReflection.Build();

                if (!partitions.HasValue && consumer is not null)
                {
                    string topic = consumer.Subscription.FirstOrDefault();

                    partitions = partitionsDict.ContainsKey(topic)
                        ? partitionsDict[topic]
                        : default;
                }
            }
            while (++consumers < partitions);
        }

        public static void ApplyConsumerPerQuantity(IServiceProvider provider, Type consumerType, int quantity, bool test = false)
        {
            var kafkaReflection = KafkaReflection.CreateInstance(provider, consumerType, test);

            foreach (int _ in Enumerable.Range(0, quantity))
                kafkaReflection.Build();
        }
    }
}