using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Reactive.Kafka.Interfaces;
using System.Reflection;

namespace Reactive.Kafka.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddReactiveKafkaConsumer(this IServiceCollection services, string bootstrapServer, string groupId = default)
        {
            return services.AddReactiveKafkaConsumer(config =>
            {
                config.BootstrapServers = bootstrapServer;
                config.GroupId = groupId;
            }, Assembly.GetCallingAssembly());
        }

        public static IServiceCollection AddReactiveKafkaConsumer(this IServiceCollection services, Action<ConsumerConfig> customize, Assembly assembly = default)
        {
            ConsumerConfig config = new();
            customize(config);

            services.AddSingleton(config);

            assembly ??= Assembly.GetCallingAssembly();

            ApplyConsumersFromAssembly2(
                services.BuildServiceProvider(), assembly);

            return services;
        }

        #region Non-Public Methods        
        private static void ApplyConsumersFromAssembly2(IServiceProvider provider, Assembly assembly)
        {
            IEnumerable<Type> types = assembly
                .GetTypes()
                .Where(type => type.GetInterface(typeof(IKafkaConsumer<>).Name, true) is not null);

            var config = provider.GetRequiredService<ConsumerConfig>();

            foreach (Type type in types)
            {
                object consumerInstance = ActivatorUtilities
                    .CreateInstance(provider, type);

                Type genericTypeArgumentMessage = type
                    .GetInterface(typeof(IKafkaConsumer<>).Name, true)?
                    .GenericTypeArguments[0];

                if (genericTypeArgumentMessage is null) continue;

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
            }
        }
        #endregion
    }
}