using System.Reflection;
using System.Text.Json;
using Confluent.Kafka;
using Reactive.Kafka.Attributes;

namespace Reactive.Kafka.Client
{
    public class ReactiveKafkaClient
    {

        public static void Run(Func<FluentClientConfig, FluentClientConfig> action) {

            var globalConfig = action(new FluentClientConfig());

            var types = Assembly.GetCallingAssembly()
                .GetTypes()
                .Where(x => x.IsDefined(typeof(ReactiveKafkaConsumerAttribute)));

            if (types == null) {
                throw new NotImplementedException("Couldn't find a consumer with 'ReactiveKafkaConsumerAttribute'");
            }

            foreach(var type in types) {
                
                var configMethodInfo = type
                    .GetMethods()
                    .FirstOrDefault(x => x.IsDefined(typeof(ConsumerConfigAttribute)));

                if (globalConfig.ConsumerConfig == null && configMethodInfo == null) {
                    throw new NotImplementedException("Couldn't find a consumer configuration 'ConsumerConfigAttribute'");
                }

                var listenerMethodInfo = type
                    .GetMethods()
                    .FirstOrDefault(x => x.IsDefined(typeof(ConsumerListenerAttribute)));

                if (listenerMethodInfo == null) {
                    throw new NotImplementedException("Couldn't find a consumer listener 'ConsumerListenerAttribute'");
                }

                var consumerInstance = Activator.CreateInstance(type);

                var config = globalConfig.ConsumerConfig;

                if (configMethodInfo != null) {
                    config = configMethodInfo.Invoke(consumerInstance, null) as ConsumerConfig;
                }

                if (config == null) {
                    throw new NotImplementedException("Couldn't find a consumer configuration");
                }

                Console.WriteLine($"created consumer '{config.ClientId}'");

                var listenerAttribute = listenerMethodInfo
                    .GetCustomAttributes()
                    .FirstOrDefault(x => x is ConsumerListenerAttribute) as ConsumerListenerAttribute;

                if (listenerAttribute == null || !listenerAttribute.Topics.Any()) {
                    throw new NotImplementedException($"Could'nt find a topic confiugration for consumer '{config.ClientId}'");
                }

                using (var consumer = new ConsumerBuilder<Ignore, string>(config).Build()) {

                    consumer.Subscribe(listenerAttribute.Topics);
                    var topicsString = $"[{string.Join(", ", listenerAttribute.Topics).Trim()}]";
                    Console.WriteLine($"subscribing consumer '{config.ClientId}' to the topics '{topicsString}'");
                                        
                    while (true) {
                        var consumeResult = consumer.Consume();

                        if (consumeResult != null)
                        {
                            Type typeToDeserialize = listenerAttribute.DeserializeTo;
                            var messageValue = consumeResult?.Message?.Value ?? string.Empty;
                            var messageDeserialized = typeToDeserialize != typeof(string) 
                                ? JsonSerializer.Deserialize(messageValue, typeToDeserialize)
                                : messageValue;

                            if (messageDeserialized != null) {
                                var parameters = new object[] { messageDeserialized };
                                listenerMethodInfo.Invoke(consumerInstance, parameters);
                            }
                        }
                    }
                }

            }

        }
    }
}